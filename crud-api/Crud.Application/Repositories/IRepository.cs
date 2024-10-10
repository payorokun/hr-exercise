namespace Crud.Application.Repositories;

public interface IRepositoryWrite<in TEntity>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task ClearQuotes();
}
public interface IRepository<TEntity> : IRepositoryWrite<TEntity>
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
}
