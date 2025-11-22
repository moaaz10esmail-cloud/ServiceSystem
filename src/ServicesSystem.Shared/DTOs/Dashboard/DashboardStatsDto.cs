namespace ServicesSystem.Shared.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int CompletedRequests { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalTechnicians { get; set; }
    public int TotalServices { get; set; }
}
