using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.ValueObjects;

namespace ServicesSystem.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? AvatarUrl { get; set; }
    public Location? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal Rating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;

    // Authentication Properties
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public DateTime? LastPasswordChangeDate { get; set; }

    // Navigation Properties
    public virtual ICollection<ServiceRequest> CustomerRequests { get; set; } = new List<ServiceRequest>();
    public virtual ICollection<ServiceRequest> TechnicianRequests { get; set; } = new List<ServiceRequest>();
    public virtual Wallet? Wallet { get; set; }
    public virtual ICollection<Review> GivenReviews { get; set; } = new List<Review>();
    public virtual ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public string FullName => $"{FirstName} {LastName}";
}
