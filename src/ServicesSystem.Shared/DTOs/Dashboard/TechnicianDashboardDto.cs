namespace ServicesSystem.Shared.DTOs.Dashboard;

public class TechnicianDashboardDto
{
    public int TotalAssignedRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int InProgressRequests { get; set; }
    public decimal TotalEarnings { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
