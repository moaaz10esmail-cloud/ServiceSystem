namespace ServicesSystem.Shared.DTOs.Payments;

public class CreatePaymentDto
{
    public Guid RequestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentGateway { get; set; }
}
