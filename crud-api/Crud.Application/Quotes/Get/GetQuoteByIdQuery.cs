using Crud.Application.Dtos;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.Get;
public record GetQuoteByIdQuery(int Id) : IRequest<QuoteDto>;