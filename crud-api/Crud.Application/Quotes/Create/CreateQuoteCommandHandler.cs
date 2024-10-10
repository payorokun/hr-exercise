using AutoMapper;
using Crud.Application.Dtos;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.Create;

public class CreateQuoteCommandHandler(IUnitOfWork unitOfWork, IRepository<Quote> repository, IMapper mapper) : IRequestHandler<CreateQuoteCommand, QuoteDto>
{
    public async Task<QuoteDto> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<Quote>(request.NewItem);
        await unitOfWork.CreateTransaction()
            .WithRepo(repository)
            .Add(entity)
            .Ready()
            .CommitAsync();
        
        return mapper.Map<QuoteDto>(entity);
    }
}
