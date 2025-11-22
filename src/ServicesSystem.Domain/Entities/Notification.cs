using ServicesSystem.Domain.Enums;

namespace ServicesSystem.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public string? ActionUrl { get; set; }
    public string? Data { get; set; } // JSON data

    // Foreign Keys
    public Guid UserId { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
}
