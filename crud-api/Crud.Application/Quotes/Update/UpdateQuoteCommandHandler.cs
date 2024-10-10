using AutoMapper;
using Crud.Application.Dtos;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crud.Application.Quotes.Update;
internal class UpdateQuoteCommandHandler(IUnitOfWork unitOfWork, IRepository<Quote> repository, IMapper mapper) : IRequestHandler<UpdateQuoteCommand, QuoteDto>
{
    public async Task<QuoteDto> Handle(UpdateQuoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.UpdatedQuote.Id);
            await unitOfWork.CreateTransaction()
                .WithRepo(repository)
                .Update(entity)
                .Ready()
                .CommitAsync();
            return mapper.Map<QuoteDto>(entity);
        }
        catch (DbUpdateConcurrencyException e)
        {
            return null;
        }
    }
}
