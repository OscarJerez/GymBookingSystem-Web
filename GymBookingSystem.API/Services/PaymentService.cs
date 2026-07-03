using GymBookingSystem.API.Domain;

namespace GymBookingSystem.API.Services;

public interface IPaymentService
{
    Task<MembershipSubscription> CreateMembershipSubscriptionAsync(int userId, int planId, string paymentMethod);
    Task<bool> VerifyPaymentAsync(string paymentIntentId);
    Task<MembershipSubscription?> GetActiveSubscriptionAsync(int userId);
    Task<bool> CancelSubscriptionAsync(int subscriptionId);
    Task<bool> RenewSubscriptionAsync(int subscriptionId, string paymentMethod);
}

public class PaymentService : IPaymentService
{
    private readonly GymBookingDbContext _context;
    private readonly ILogger<PaymentService> _logger;
    private readonly IConfiguration _config;

    public PaymentService(GymBookingDbContext context, ILogger<PaymentService> logger, IConfiguration config)
    {
        _context = context;
        _logger = logger;
        _config = config;
    }

    public async Task<MembershipSubscription> CreateMembershipSubscriptionAsync(int userId, int planId, string paymentMethod)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var plan = await _context.MembershipPlans.FindAsync(planId);
            if (plan == null)
                throw new InvalidOperationException("Plan not found");

            // Check if user already has active subscription
            var activeSubscription = await _context.MembershipSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active && !s.IsExpired);
            if (activeSubscription != null)
                throw new InvalidOperationException("User already has an active subscription");

            // TODO: Process payment with Stripe
            // For now, create mock payment
            var paymentIntentId = GeneratePaymentIntentId();

            var subscription = new MembershipSubscription
            {
                UserId = userId,
                PlanId = planId,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                Status = SubscriptionStatus.Active,
                AmountPaid = plan.Price,
                PaymentIntentId = paymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.MembershipSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subscription created for user {userId}, plan {planId}");
            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating subscription: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> VerifyPaymentAsync(string paymentIntentId)
    {
        try
        {
            // TODO: Verify with Stripe API
            // For now, all payments are valid
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error verifying payment: {ex.Message}");
            return false;
        }
    }

    public async Task<MembershipSubscription?> GetActiveSubscriptionAsync(int userId)
    {
        try
        {
            return await _context.MembershipSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active && !s.IsExpired);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting active subscription: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
    {
        try
        {
            var subscription = await _context.MembershipSubscriptions.FindAsync(subscriptionId);
            if (subscription == null)
                throw new InvalidOperationException("Subscription not found");

            subscription.Status = SubscriptionStatus.Cancelled;
            _context.MembershipSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subscription {subscriptionId} cancelled");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling subscription: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RenewSubscriptionAsync(int subscriptionId, string paymentMethod)
    {
        try
        {
            var subscription = await _context.MembershipSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription == null)
                throw new InvalidOperationException("Subscription not found");

            if (subscription.Plan == null)
                throw new InvalidOperationException("Plan not found");

            subscription.StartDate = DateTime.UtcNow;
            subscription.ExpiryDate = DateTime.UtcNow.AddDays(subscription.Plan.DurationDays);
            subscription.Status = SubscriptionStatus.Active;
            subscription.PaymentIntentId = GeneratePaymentIntentId();

            _context.MembershipSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subscription {subscriptionId} renewed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error renewing subscription: {ex.Message}");
            throw;
        }
    }

    private string GeneratePaymentIntentId() => $"pi_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24)}";
}
