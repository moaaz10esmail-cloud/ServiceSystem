namespace ServicesSystem.Shared.DTOs.Tracking;

public class CreateTrackingDto
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
