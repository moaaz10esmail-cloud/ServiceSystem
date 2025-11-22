using Microsoft.EntityFrameworkCore;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Infrastructure.Data;

namespace ServicesSystem.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Request)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<Review?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Request)
            .FirstOrDefaultAsync(r => r.RequestId == requestId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Request)
            .Where(r => r.TechnicianId == technicianId && r.IsPublic && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Technician)
            .Include(r => r.Request)
            .Where(r => r.CustomerId == customerId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        return review;
    }

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await GetByIdAsync(id, cancellationToken);
        if (review != null)
        {
            review.IsDeleted = true;
            _context.Reviews.Update(review);
        }
    }
}
