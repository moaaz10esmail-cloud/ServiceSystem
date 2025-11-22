using Microsoft.EntityFrameworkCore;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Infrastructure.Data;

namespace ServicesSystem.Infrastructure.Repositories;

public class RequestTrackingRepository : IRequestTrackingRepository
{
    private readonly ApplicationDbContext _context;

    public RequestTrackingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RequestTracking>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.RequestTrackings
            .Where(t => t.RequestId == requestId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<RequestTracking?> GetLatestByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.RequestTrackings
            .Where(t => t.RequestId == requestId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RequestTracking> AddAsync(RequestTracking tracking, CancellationToken cancellationToken = default)
    {
        await _context.RequestTrackings.AddAsync(tracking, cancellationToken);
        return tracking;
    }
}
