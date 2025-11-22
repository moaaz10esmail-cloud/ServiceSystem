using Microsoft.EntityFrameworkCore.Storage;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Infrastructure.Data;

namespace ServicesSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository users,
        IServiceRepository services,
        IRequestRepository requests,
        IRefreshTokenRepository refreshTokens,
        IRequestTrackingRepository trackings,
        IPaymentRepository payments,
        IReviewRepository reviews)
    {
        _context = context;
        Users = users;
        Services = services;
        Requests = requests;
        RefreshTokens = refreshTokens;
        Trackings = trackings;
        Payments = payments;
        Reviews = reviews;
    }

    public IUserRepository Users { get; }
    public IServiceRepository Services { get; }
    public IRequestRepository Requests { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IRequestTrackingRepository Trackings { get; }
    public IPaymentRepository Payments { get; }
    public IReviewRepository Reviews { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
