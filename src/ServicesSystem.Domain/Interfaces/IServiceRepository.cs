using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Domain.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Service> AddAsync(Service service, CancellationToken cancellationToken = default);
    Task UpdateAsync(Service service, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
