using AutoMapper;
using Crud.Application.Dtos;
using Crud.Application.Repositories;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.Get;
internal class GetQuoteByIdQueryHandler(IRepository<Quote> repository, IMapper mapper) : IRequestHandler<GetQuoteByIdQuery, QuoteDto>
{
    public async Task<QuoteDto> Handle(GetQuoteByIdQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<QuoteDto>(await repository.GetByIdAsync(request.Id));
    }
}
