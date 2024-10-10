namespace Crud.Application.UnitOfWork;
public interface IUnitOfWork
{
    ITransactionScope CreateTransaction();
}