namespace GymBookingSystem.API.DTOs;

public class MembershipDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateMembershipRequest
{
    public string Type { get; set; } = string.Empty; // "Basic", "Premium", "VIP"
}

public class PaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CreatePaymentRequest
{
    public int? MembershipId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty; // "CreditCard", "PayPal", etc.
    public string? StripeToken { get; set; } // For Stripe integration
}

public class MembershipPlanDto
{
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ClassesPerMonth { get; set; }
    public string[] Benefits { get; set; } = Array.Empty<string>();
}
