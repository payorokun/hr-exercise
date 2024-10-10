using Crud.Application;
using Crud.Application.Dtos;
using Crud.Application.Quotes.Create;
using Crud.Application.Quotes.Delete;
using Crud.Application.Quotes.Get;
using Crud.Application.Quotes.List;
using Crud.Application.Quotes.PairCount;
using Crud.Application.Quotes.ProcessQuotesFile;
using Crud.Application.Quotes.Update;
using Crud.Infrastructure;
using Crud.Infrastructure.Data;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});

var app = builder.Build();

app.Services.EnsureDatabaseCreated();

app.MapPost("/quotes", async (IMediator mediator, CreateUpdateQuoteDto newItem) =>
{
    var command = new CreateQuoteCommand(newItem);
    var quote = await mediator.Send(command);
    return Results.Created($"/quotes/{quote.Id}", quote);
});

app.MapGet("/quotes/{id}", async (IMediator mediator, int id) =>
{
    var quote = await mediator.Send(new GetQuoteByIdQuery(id));
    return quote is not null ? Results.Ok(quote) : Results.NotFound();
});

app.MapGet("/quotes", async (IMediator mediator) =>
{
    var quote = await mediator.Send(new GetQuotesQuery());
    return quote is not null ? Results.Ok(quote) : Results.NotFound();
});

app.MapPut("/quotes/{id}", async (IMediator mediator, int id, CreateUpdateQuoteDto updatedItem) =>
{
    var command = new UpdateQuoteCommand(id, updatedItem);
    if (id != command.UpdatedQuote.Id) return Results.BadRequest();

    var updatedQuote = await mediator.Send(command);
    return updatedQuote is not null ? Results.Ok(updatedQuote) : Results.NotFound();
});

app.MapDelete("/quotes/{id}", async (IMediator mediator, int id) =>
{
    await mediator.Send(new DeleteQuoteCommand(id));
    return Results.NoContent();
});

app.MapGet("/quotes/matches/{length}", async (IMediator mediator, int length) =>
{
    var pairs = await mediator.Send(new GetQuotePairCountByLengthQuery(length));
    return pairs is not null ? Results.Ok(pairs) : Results.NotFound();
});

app.MapPost("/quotes/upload", async (IMediator mediator, IFormFile file) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }

    var command = new ProcessQuotesFileCommand(file.OpenReadStream());
    await mediator.Send(command);
    return Results.Ok();
}).DisableAntiforgery();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
