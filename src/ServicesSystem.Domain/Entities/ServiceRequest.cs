using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.ValueObjects;

namespace ServicesSystem.Domain.Entities;

public class ServiceRequest : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public Address CustomerLocation { get; set; } = null!;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public decimal TotalAmount { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? RejectionReason { get; set; }

    // Foreign Keys
    public Guid CustomerId { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid ServiceId { get; set; }

    // Navigation Properties
    public virtual User Customer { get; set; } = null!;
    public virtual User? Technician { get; set; }
    public virtual Service Service { get; set; } = null!;
    public virtual Payment? Payment { get; set; }
    public virtual Review? Review { get; set; }
    public virtual ICollection<RequestTracking> Trackings { get; set; } = new List<RequestTracking>();

    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue 
        ? CompletedAt.Value - StartedAt.Value 
        : null;
}
