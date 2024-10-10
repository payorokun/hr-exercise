using Crud.Application.Dtos;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.Create;

public record CreateQuoteCommand(CreateUpdateQuoteDto NewItem) : IRequest<QuoteDto>;
