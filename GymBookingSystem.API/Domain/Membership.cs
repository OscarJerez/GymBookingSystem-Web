namespace GymBookingSystem.API.Domain;

public class MembershipPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; } // 30, 90, 365
    public int ClassesPerMonth { get; set; } // Unlimited if 0
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MembershipSubscription> Subscriptions { get; set; } = new List<MembershipSubscription>();
}

public class MembershipSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public decimal AmountPaid { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
    public MembershipPlan? Plan { get; set; }

    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
}

public enum SubscriptionStatus
{
    Active,
    Expired,
    Cancelled,
    Paused
}
