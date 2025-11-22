using Microsoft.EntityFrameworkCore;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Infrastructure.Data;

namespace ServicesSystem.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Customer)
            .Include(p => p.Request)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<Payment?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Customer)
            .Include(p => p.Request)
            .FirstOrDefaultAsync(p => p.RequestId == requestId && !p.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Customer)
            .Include(p => p.Request)
            .Where(p => p.CustomerId == customerId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
        return payment;
    }

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(payment);
        return Task.CompletedTask;
    }
}
