using AutoMapper;
using Crud.Application.Dtos;
using Crud.Application.Repositories;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.List;
internal class GetQuotesQueryHandler(IRepository<Quote> repository, IMapper mapper) : IRequestHandler<GetQuotesQuery, List<QuoteDto>>
{
    public async Task<List<QuoteDto>> Handle(GetQuotesQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<List<QuoteDto>>((await repository.GetAllAsync()).ToList());
    }
}
