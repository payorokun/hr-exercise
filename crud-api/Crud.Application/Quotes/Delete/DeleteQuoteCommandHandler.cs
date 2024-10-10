using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crud.Application.Quotes.Delete;
public class DeleteQuoteCommandHandler(IUnitOfWork unitOfWork, IRepository<Quote> repository) : IRequestHandler<DeleteQuoteCommand>
{
    public async Task Handle(DeleteQuoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CreateTransaction()
                .WithRepo(repository)
                .Delete(new Quote { Id = request.Id})
                .Ready()
                .CommitAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            //do nothing if not found
        }
    }
}
