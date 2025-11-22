namespace ServicesSystem.Shared.DTOs.Tracking;

public class RequestTrackingDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public LocationDto? CurrentLocation { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RequestId { get; set; }
}
