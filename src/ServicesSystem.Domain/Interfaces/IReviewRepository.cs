using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Review?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
