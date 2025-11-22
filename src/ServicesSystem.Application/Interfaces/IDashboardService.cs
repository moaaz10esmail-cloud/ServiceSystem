using ServicesSystem.Shared.DTOs.Dashboard;

namespace ServicesSystem.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
    Task<TechnicianDashboardDto> GetTechnicianDashboardAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<CustomerDashboardDto> GetCustomerDashboardAsync(Guid customerId, CancellationToken cancellationToken = default);
}
