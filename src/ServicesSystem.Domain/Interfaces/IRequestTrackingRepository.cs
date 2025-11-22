using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Domain.Interfaces;

public interface IRequestTrackingRepository
{
    Task<IEnumerable<RequestTracking>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<RequestTracking?> GetLatestByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<RequestTracking> AddAsync(RequestTracking tracking, CancellationToken cancellationToken = default);
}
