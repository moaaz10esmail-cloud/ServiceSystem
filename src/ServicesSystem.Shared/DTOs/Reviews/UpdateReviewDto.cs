namespace ServicesSystem.Shared.DTOs.Reviews;

public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsPublic { get; set; } = true;
}
