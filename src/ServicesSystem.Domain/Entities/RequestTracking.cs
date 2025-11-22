using ServicesSystem.Domain.ValueObjects;

namespace ServicesSystem.Domain.Entities;

public class RequestTracking : BaseEntity
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Location? CurrentLocation { get; set; }

    // Foreign Keys
    public Guid RequestId { get; set; }

    // Navigation Properties
    public virtual ServiceRequest Request { get; set; } = null!;
}
