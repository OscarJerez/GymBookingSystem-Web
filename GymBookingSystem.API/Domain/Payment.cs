namespace GymBookingSystem.API.Domain;

public class Membership
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public MembershipType Type { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public string PaymentId { get; set; } = string.Empty; // Stripe/Payment provider ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}

public enum MembershipType
{
    Basic,      // $29/month - 5 classes/month
    Premium,    // $79/month - Unlimited classes
    VIP         // $129/month - Unlimited + personal trainer
}

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? MembershipId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string TransactionId { get; set; } = string.Empty; // From Stripe/provider
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    // Navigation
    public User? User { get; set; }
    public Membership? Membership { get; set; }
}

public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    PayPal,
    ApplePay,
    GooglePay
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
