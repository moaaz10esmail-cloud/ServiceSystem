namespace ServicesSystem.Shared.DTOs.Dashboard;

public class CustomerDashboardDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int CompletedRequests { get; set; }
    public decimal TotalSpent { get; set; }
    public int ReviewsGiven { get; set; }
}
