namespace ServicesSystem.Shared.DTOs.Services;

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int EstimatedDuration { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
}
