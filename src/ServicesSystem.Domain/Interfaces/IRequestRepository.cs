using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;

namespace ServicesSystem.Domain.Interfaces;

public interface IRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceRequest>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceRequest>> GetByTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceRequest>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default);
    Task<ServiceRequest> AddAsync(ServiceRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
