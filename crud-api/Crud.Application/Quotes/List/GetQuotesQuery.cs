using Crud.Application.Dtos;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.List;

public record GetQuotesQuery : IRequest<List<QuoteDto>>;
