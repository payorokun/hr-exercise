using MediatR;

namespace Crud.Application.Quotes.PairCount;
public record GetQuotePairCountByLengthQuery(int MaxLength) : IRequest<QuotePairCountQueryResult>;