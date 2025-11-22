using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Application.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Dashboard;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("admin")]
    public async Task<ActionResult<Result<DashboardStatsDto>>> GetAdminDashboard()
    {
        var stats = await _dashboardService.GetAdminDashboardAsync();
        return Ok(Result<DashboardStatsDto>.Success(stats));
    }

    [HttpGet("technician/{technicianId}")]
    public async Task<ActionResult<Result<TechnicianDashboardDto>>> GetTechnicianDashboard(Guid technicianId)
    {
        var stats = await _dashboardService.GetTechnicianDashboardAsync(technicianId);
        return Ok(Result<TechnicianDashboardDto>.Success(stats));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<Result<CustomerDashboardDto>>> GetCustomerDashboard(Guid customerId)
    {
        var stats = await _dashboardService.GetCustomerDashboardAsync(customerId);
        return Ok(Result<CustomerDashboardDto>.Success(stats));
    }
}
