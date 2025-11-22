namespace ServicesSystem.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public bool IsPublic { get; set; } = true;

    // Foreign Keys
    public Guid RequestId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TechnicianId { get; set; }

    // Navigation Properties
    public virtual ServiceRequest Request { get; set; } = null!;
    public virtual User Customer { get; set; } = null!;
    public virtual User Technician { get; set; } = null!;
}
