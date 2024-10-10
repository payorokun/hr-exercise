using Crud.Application.Repositories;

namespace Crud.Application.UnitOfWork;

public interface ITransactionScope
{
    ITransactionScopeWithRepo<TEntity> WithRepo<TEntity>(
        IRepositoryWrite<TEntity> repository);

    ITransactionScopeWithBatchRepo<TEntity> WithBatchRepo<TEntity>(
        IRepositoryForBatch<TEntity> repository);
}

public interface ITransactionBuilder
{
    void AppendAction(Action action);
}

public interface ITransactionScopeWithRepo<in TEntity>
{
    ITransactionScopeReady Clear();
    ITransactionScopeWithChanges<TEntity> Add(TEntity item);
    
    ITransactionScopeWithChanges<TEntity> Update(TEntity item);
    ITransactionScopeWithChanges<TEntity> Delete(TEntity item);
}

public interface ITransactionScopeWithBatchRepo<in TEntity>
{
    ITransactionScopeComplete AddBatch(IEnumerable<TEntity> batch);
}

public interface ITransactionScopeComplete
{
    ITransactionScopeReady Ready();
}
public interface ITransactionScopeWithChanges<in TEntity> : ITransactionScopeWithRepo<TEntity>, ITransactionScopeComplete
{
    ITransactionScopeWithRepo<TOtherEntity> AndForRepo<TOtherEntity>(
        IRepositoryWrite<TOtherEntity> otherRepository);
}

public interface ITransactionScopeReady : ITransactionScope
{
    Task CommitAsync();
}