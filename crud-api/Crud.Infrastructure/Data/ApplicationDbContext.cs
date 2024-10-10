using Crud.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Crud.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options), IApplicationDbContext, IApplicationDbContextForBatch
    {
        public DbSet<Quote> Quotes { get; set; }
        private bool? _isInMemory;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public bool IsInMemory
        {
            get
            {
                _isInMemory ??= Database.ProviderName!.Equals("Microsoft.EntityFrameworkCore.InMemory");

                return _isInMemory.Value;
            }
        }

        public async Task ClearQuotesAsync()
        {
            await Database.ExecuteSqlRawAsync("DELETE FROM Quotes");
            foreach (var entityEntry in ChangeTracker.Entries<Quote>())
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        public Task SaveBatchAsync<TEntity>(IEnumerable<TEntity> batch) where TEntity:class
        {
            if (IsInMemory)
            {
                return Set<TEntity>().AddRangeAsync(batch);
            }
            else
            {
                return this.BulkInsertAsync(batch);
            }
        }

        public Task BeginTransactionAsync()
        {
            if (IsInMemory) return Task.CompletedTask;
            return Database.BeginTransactionAsync();
        }

        public Task CommitTransactionAsync()
        {
            if (IsInMemory) return Task.CompletedTask;
            return Database.CommitTransactionAsync();
        }

        public Task RollbackTransactionAsync()
        {
            if (IsInMemory) return Task.CompletedTask;
            return Database.RollbackTransactionAsync();
        }
    }

    public interface IApplicationDbContext
    {
        bool IsInMemory { get; }
        DbSet<Quote> Quotes { get; set; }
        Task ClearQuotesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }

    public interface IApplicationDbContextForBatch : IApplicationDbContext
    {
        Task SaveBatchAsync<TEntity>(IEnumerable<TEntity> batch) where TEntity : class;
    }
}
