using Crud.Application.Repositories;
using Crud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crud.Infrastructure.Repositories;

public class GeneralRepository<TEntity>(IApplicationDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    public async Task<TEntity> GetByIdAsync(int id) => await context.Set<TEntity>().FindAsync(id);
    
    public async Task<IEnumerable<TEntity>> GetAllAsync() => await context.Set<TEntity>().ToListAsync();

    public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public void Delete(TEntity entity)=> context.Set<TEntity>().Remove(entity);
    public async Task ClearQuotes() => await context.ClearQuotesAsync();
}

public class BatchEnabledRepository<TEntity>(IApplicationDbContextForBatch context) : GeneralRepository<TEntity>(context), 
    IRepositoryForBatch<TEntity>
    where TEntity : class
{
    public async Task SaveBatchAsync(IEnumerable<TEntity> batch)
    {
        await context.SaveBatchAsync(batch);
    }
}