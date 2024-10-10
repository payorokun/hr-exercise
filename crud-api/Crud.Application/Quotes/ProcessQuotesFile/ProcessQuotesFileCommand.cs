using MediatR;

namespace Crud.Application.Quotes.ProcessQuotesFile;

public record ProcessQuotesFileCommand(Stream FileStream) : IRequest;
