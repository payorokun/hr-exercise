using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Infrastructure.Data;

namespace Crud.Infrastructure.UnitOfWork;
public class UnitOfWork(IApplicationDbContext dbContext) : IUnitOfWork
{
    private class TransactionScope(IApplicationDbContext dbContext) : 
        ITransactionScope, ITransactionScopeReady, ITransactionBuilder
    {
        private readonly List<Action> _actions = new();
        public ITransactionScopeWithRepo<TEntity> WithRepo<TEntity>(IRepositoryWrite<TEntity> repository)
        {
            return new TransactionScopeForRepository<TEntity>(
                repository, 
                this as ITransactionScopeReady, 
                this as ITransactionBuilder);
        }

        void ITransactionBuilder.AppendAction(Action action)
        {
            _actions.Add(action);
        }

        async Task ITransactionScopeReady.CommitAsync()
        {
            await dbContext.BeginTransactionAsync();
            try
            {
                await Execute();
                await dbContext.CommitTransactionAsync();
                return;
            }
            catch (Exception e)
            {
                await dbContext.RollbackTransactionAsync();
                throw;

            }
        }

        private async Task Execute()
        {
            foreach (var action in _actions)
            {
                action();
            }
            await dbContext.SaveChangesAsync();
        }

        private class TransactionScopeForRepository<TEntity>(
            IRepositoryWrite<TEntity> repository,
            ITransactionScopeReady transactionScopeReady,
            ITransactionBuilder transactionBuilder) : ITransactionScopeWithRepo<TEntity>
        {
            public ITransactionScopeWithRepo<TOtherEntity> AndForRepo<TOtherEntity>(
                IRepositoryWrite<TOtherEntity> otherRepository)
            {
                return new TransactionScopeForRepository<TOtherEntity>(
                    otherRepository, transactionScopeReady, transactionBuilder);
            }

            public ITransactionScopeWithRepo<TEntity> Add(TEntity item)
            {
                transactionBuilder.AppendAction(()=> repository.Add(item));
                return this;
            }

            public ITransactionScopeWithRepo<TEntity> Update(TEntity item)
            {
                transactionBuilder.AppendAction(() => repository.Update(item));
                return this;
            }

            public ITransactionScopeWithRepo<TEntity> Delete(TEntity item)
            {
                transactionBuilder.AppendAction(() => repository.Delete(item));
                return this;
            }

            public ITransactionScopeReady Ready()
            {
                return transactionScopeReady;
            }
        }
    }


    public ITransactionScope CreateTransaction()
    {
        return new TransactionScope(dbContext);
    }
}
