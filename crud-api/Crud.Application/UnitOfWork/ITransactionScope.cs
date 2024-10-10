using Crud.Application.Repositories;

namespace Crud.Application.UnitOfWork;

public interface ITransactionScope
{
    ITransactionScopeWithRepo<TEntity> WithRepo<TEntity>(
        IRepositoryWrite<TEntity> repository);
}

public interface ITransactionBuilder
{
    void AppendAction(Action action);
}
public interface ITransactionScopeWithRepo<in TEntity>
{
    ITransactionScopeWithRepo<TOtherEntity> AndForRepo<TOtherEntity>(
        IRepositoryWrite<TOtherEntity> otherRepository);
    ITransactionScopeWithRepo<TEntity> Add(TEntity item);
    ITransactionScopeWithRepo<TEntity> Update(TEntity item);
    ITransactionScopeWithRepo<TEntity> Delete(TEntity item);
    ITransactionScopeReady Ready();
}

public interface ITransactionScopeReady : ITransactionScope
{
    Task CommitAsync();
}