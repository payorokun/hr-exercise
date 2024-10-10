using MediatR;

namespace Crud.Application.Quotes.Delete;

public record DeleteQuoteCommand(int Id) : IRequest;
