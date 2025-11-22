using Microsoft.EntityFrameworkCore;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;

namespace ServicesSystem.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly Data.ApplicationDbContext _context;

    public ServiceRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Category)
            .Where(s => !s.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Category)
            .Where(s => s.CategoryId == categoryId && !s.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Service> AddAsync(Service service, CancellationToken cancellationToken = default)
    {
        await _context.Services.AddAsync(service, cancellationToken);
        return service;
    }

    public Task UpdateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _context.Services.Update(service);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await GetByIdAsync(id, cancellationToken);
        if (service != null)
        {
            service.IsDeleted = true;
            _context.Services.Update(service);
        }
    }
}
