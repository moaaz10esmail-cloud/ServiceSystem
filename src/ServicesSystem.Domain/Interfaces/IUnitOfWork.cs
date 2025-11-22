namespace ServicesSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IServiceRepository Services { get; }
    IRequestRepository Requests { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IRequestTrackingRepository Trackings { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
