using ServicesSystem.Domain.Enums;

namespace ServicesSystem.Domain.Entities;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }

    // Foreign Keys
    public Guid RequestId { get; set; }
    public Guid CustomerId { get; set; }

    // Navigation Properties
    public virtual ServiceRequest Request { get; set; } = null!;
    public virtual User Customer { get; set; } = null!;
}
