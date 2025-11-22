namespace ServicesSystem.Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int EstimatedDuration { get; set; } // in minutes
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign Keys
    public Guid CategoryId { get; set; }

    // Navigation Properties
    public virtual ServiceCategory Category { get; set; } = null!;
    public virtual ICollection<ServiceRequest> Requests { get; set; } = new List<ServiceRequest>();
}
