using Microsoft.EntityFrameworkCore;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;

namespace ServicesSystem.Infrastructure.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly Data.ApplicationDbContext _context;

    public RequestRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Service)
            .Include(r => r.Payment)
            .Include(r => r.Review)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Service)
            .Where(r => !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Technician)
            .Include(r => r.Service)
            .Include(r => r.Payment)
            .Where(r => r.CustomerId == customerId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> GetByTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Customer)
            .Include(r => r.Service)
            .Include(r => r.Payment)
            .Where(r => r.TechnicianId == technicianId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Service)
            .Where(r => r.Status == status && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceRequest> AddAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        await _context.ServiceRequests.AddAsync(request, cancellationToken);
        return request;
    }

    public Task UpdateAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        _context.ServiceRequests.Update(request);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await GetByIdAsync(id, cancellationToken);
        if (request != null)
        {
            request.IsDeleted = true;
            _context.ServiceRequests.Update(request);
        }
    }
}
