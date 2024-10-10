using AutoMapper;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Domain.Models;
using MediatR;
using System.Text.Json;
using Crud.Application.Dtos;

namespace Crud.Application.Quotes.ProcessQuotesFile;

public class ProcessQuotesFileCommandHandler(IUnitOfWork unitOfWork, IRepository<Quote> repository, IMapper mapper) : IRequestHandler<ProcessQuotesFileCommand>
{
    public async Task Handle(ProcessQuotesFileCommand request, CancellationToken cancellationToken)
    {
        await ProcessJsonFromStreamAsync(request.FileStream);
    }

    //read stream in batches
    private async Task ProcessJsonFromStreamAsync(Stream fileStream)
    {
        var batch = new List<Quote>();
        const int batchSize = 1000;
        using var streamReader = new StreamReader(fileStream);
        using var jsonDocument = await JsonDocument.ParseAsync(fileStream);

        int currentBatchCount = 0;

        foreach (var element in jsonDocument.RootElement.EnumerateArray())
        {
            // Deserialize the individual item
            var item = JsonSerializer.Deserialize<CreateUpdateQuoteDto>(element.GetRawText());
            if (item == null) continue;

            batch.Add(mapper.Map<Quote>(item));
            currentBatchCount++;

            // When batch size is reached, save to DB
            if (currentBatchCount != batchSize) continue;
            await SaveBatchAsync(batch);
            batch.Clear();
            currentBatchCount = 0;
        }

        if (batch.Count == 0) return;

        await SaveBatchAsync(batch);
    }

    private async Task SaveBatchAsync(List<Quote> items)
    {
        var withRepo = unitOfWork.CreateTransaction()
            .WithRepo(repository);

        ITransactionScopeWithChanges<Quote> withChanges = null;
        foreach (var item in items)
        {
            withChanges = withRepo.Add(item);
        }

        if (withChanges is null) return;
        await withChanges
            .Ready()
            .CommitAsync();
    }

}
