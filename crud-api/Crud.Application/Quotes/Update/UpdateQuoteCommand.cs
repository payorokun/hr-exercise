using Crud.Application.Dtos;
using MediatR;

namespace Crud.Application.Quotes.Update;

public record UpdateQuoteCommand : IRequest<QuoteDto>
{
    public UpdateQuoteCommand(int id, CreateUpdateQuoteDto updatedListing)
    {
        UpdatedQuote = updatedListing;
        UpdatedQuote.Id = id;
    }

    public CreateUpdateQuoteDto UpdatedQuote { get; init; }
}
