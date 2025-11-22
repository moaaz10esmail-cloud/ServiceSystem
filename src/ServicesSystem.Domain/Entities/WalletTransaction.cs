namespace ServicesSystem.Domain.Entities;

public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Payment = 3,
    Refund = 4,
    Earning = 5
}

public class WalletTransaction : BaseEntity
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }

    // Foreign Keys
    public Guid WalletId { get; set; }

    // Navigation Properties
    public virtual Wallet Wallet { get; set; } = null!;
}
