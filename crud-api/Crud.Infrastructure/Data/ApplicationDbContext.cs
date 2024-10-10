using Crud.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Crud.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options), IApplicationDbContext
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

        public Task ClearQuotesAsync()
        {
            return Database.ExecuteSqlRawAsync("DELETE FROM Quotes");
        }

        public Task SaveBatchAsync<TEntity>(IEnumerable<TEntity> batch) where TEntity:class
        {
            return Set<TEntity>().AddRangeAsync(batch);
            //return this.BulkInsertAsync(batch);
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
        Task SaveBatchAsync<TEntity>(IEnumerable<TEntity> batch) where TEntity : class;
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}