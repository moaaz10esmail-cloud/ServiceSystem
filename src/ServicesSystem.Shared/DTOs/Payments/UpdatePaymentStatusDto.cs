namespace ServicesSystem.Shared.DTOs.Payments;

public class UpdatePaymentStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
}
