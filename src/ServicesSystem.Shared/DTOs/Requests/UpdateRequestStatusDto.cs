namespace ServicesSystem.Shared.DTOs.Requests;

public class UpdateRequestStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
