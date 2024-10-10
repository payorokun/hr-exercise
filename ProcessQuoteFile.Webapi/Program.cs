using Crud.Application;
using Crud.Application.Dtos;
using Crud.Application.Quotes.Create;
using Crud.Application.Quotes.Delete;
using Crud.Application.Quotes.Get;
using Crud.Application.Quotes.List;
using Crud.Application.Quotes.ProcessQuotesFile;
using Crud.Application.Quotes.Update;
using Crud.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.MapPost("/quotes/upload", async (IMediator mediator, IFormFile file) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }

    var command = new ProcessQuotesFileCommand(file.OpenReadStream());
    await mediator.Send(command);
    return Results.Ok();
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();