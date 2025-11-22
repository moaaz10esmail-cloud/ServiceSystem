namespace ServicesSystem.Shared.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsPublic { get; set; }
    public Guid RequestId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
