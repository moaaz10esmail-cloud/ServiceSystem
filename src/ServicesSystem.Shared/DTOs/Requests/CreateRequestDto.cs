namespace ServicesSystem.Shared.DTOs.Requests;

public class CreateRequestDto
{
    public string Description { get; set; } = string.Empty;
    public AddressDto CustomerLocation { get; set; } = null!;
    public DateTime ScheduledDate { get; set; }
    public Guid ServiceId { get; set; }
}
