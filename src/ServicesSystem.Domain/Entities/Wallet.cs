namespace ServicesSystem.Domain.Entities;

public class Wallet : BaseEntity
{
    public decimal Balance { get; set; } = 0;
    public string Currency { get; set; } = "USD";

    // Foreign Keys
    public Guid UserId { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}
