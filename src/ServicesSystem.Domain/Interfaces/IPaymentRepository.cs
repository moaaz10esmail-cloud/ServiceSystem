using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
}
