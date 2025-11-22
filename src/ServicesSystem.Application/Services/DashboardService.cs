using ServicesSystem.Application.Interfaces;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.DTOs.Dashboard;

namespace ServicesSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardStatsDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var allRequests = await _unitOfWork.Requests.GetAllAsync(cancellationToken);
        var allPayments = await _unitOfWork.Payments.GetByCustomerIdAsync(Guid.Empty, cancellationToken);
        var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var allServices = await _unitOfWork.Services.GetAllAsync(cancellationToken);

        // Get all payments (we'll need to modify this to get all payments)
        var completedRequests = allRequests.Where(r => r.Status == RequestStatus.Completed).ToList();
        var completedRequestIds = completedRequests.Select(r => r.Id).ToHashSet();
        
        decimal totalRevenue = 0;
        foreach (var request in completedRequests)
        {
            var payment = await _unitOfWork.Payments.GetByRequestIdAsync(request.Id, cancellationToken);
            if (payment?.Status == PaymentStatus.Completed)
            {
                totalRevenue += payment.Amount;
            }
        }

        // Calculate average rating
        var allReviews = new List<Domain.Entities.Review>();
        foreach (var user in allUsers.Where(u => u.Role == UserRole.Technician))
        {
            var reviews = await _unitOfWork.Reviews.GetByTechnicianIdAsync(user.Id, cancellationToken);
            allReviews.AddRange(reviews);
        }
        
        double averageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;

        return new DashboardStatsDto
        {
            TotalRequests = allRequests.Count(),
            PendingRequests = allRequests.Count(r => r.Status == RequestStatus.Pending),
            CompletedRequests = completedRequests.Count,
            TotalRevenue = totalRevenue,
            AverageRating = Math.Round(averageRating, 2),
            TotalCustomers = allUsers.Count(u => u.Role == UserRole.Customer),
            TotalTechnicians = allUsers.Count(u => u.Role == UserRole.Technician),
            TotalServices = allServices.Count()
        };
    }

    public async Task<TechnicianDashboardDto> GetTechnicianDashboardAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        var technicianRequests = await _unitOfWork.Requests.GetByTechnicianAsync(technicianId, cancellationToken);
        var reviews = await _unitOfWork.Reviews.GetByTechnicianIdAsync(technicianId, cancellationToken);

        var completedRequests = technicianRequests.Where(r => r.Status == RequestStatus.Completed).ToList();
        
        decimal totalEarnings = 0;
        foreach (var request in completedRequests)
        {
            var payment = await _unitOfWork.Payments.GetByRequestIdAsync(request.Id, cancellationToken);
            if (payment?.Status == PaymentStatus.Completed)
            {
                totalEarnings += payment.Amount;
            }
        }

        double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        return new TechnicianDashboardDto
        {
            TotalAssignedRequests = technicianRequests.Count(),
            CompletedRequests = completedRequests.Count,
            InProgressRequests = technicianRequests.Count(r => r.Status == RequestStatus.InProgress),
            TotalEarnings = totalEarnings,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = reviews.Count()
        };
    }

    public async Task<CustomerDashboardDto> GetCustomerDashboardAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customerRequests = await _unitOfWork.Requests.GetByCustomerAsync(customerId, cancellationToken);
        var payments = await _unitOfWork.Payments.GetByCustomerIdAsync(customerId, cancellationToken);
        var reviews = await _unitOfWork.Reviews.GetByCustomerIdAsync(customerId, cancellationToken);

        var completedPayments = payments.Where(p => p.Status == PaymentStatus.Completed);
        decimal totalSpent = completedPayments.Sum(p => p.Amount);

        return new CustomerDashboardDto
        {
            TotalRequests = customerRequests.Count(),
            PendingRequests = customerRequests.Count(r => r.Status == RequestStatus.Pending),
            CompletedRequests = customerRequests.Count(r => r.Status == RequestStatus.Completed),
            TotalSpent = totalSpent,
            ReviewsGiven = reviews.Count()
        };
    }
}
