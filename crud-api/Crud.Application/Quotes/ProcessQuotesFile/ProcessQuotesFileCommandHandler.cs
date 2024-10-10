using AutoMapper;
using Crud.Application.Repositories;
using Crud.Application.UnitOfWork;
using Crud.Domain.Models;
using MediatR;
using Crud.Application.Dtos;
using Crud.Application.Util;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Crud.Application.Cache;

namespace Crud.Application.Quotes.ProcessQuotesFile;

public class ProcessQuotesFileCommandHandler(IUnitOfWork unitOfWork, IRepository<Quote> defaultRepository, IRepositoryForBatch<Quote> batchRepository, IMapper mapper, IQuotesLengthCacheService quotesLengthCacheService) : IRequestHandler<ProcessQuotesFileCommand>
{
    public async Task Handle(ProcessQuotesFileCommand request, CancellationToken cancellationToken)
    {
        await ProcessJsonFromStreamAsync(request.FileStream);
    }

    //read stream in batches
    private async Task ProcessJsonFromStreamAsync(Stream fileStream)
    {
        await DataCleanup();

        var batch = new List<Quote>();
        const int batchSize = 1000;
        

        int currentBatchCount = 0;
        var settings = new JsonSerializerSettings
        {
            Converters = { new QuoteJsonConverter() }
        };
        using var streamReader = new StreamReader(fileStream);
        await using var jsonReader = new JsonTextReader(streamReader);
        var serializer = JsonSerializer.Create(settings);

        //individual deserialization to apply mapping rules
        while (await jsonReader.ReadAsync())
        {
            if (jsonReader.TokenType != JsonToken.StartObject)
                continue;

            // Deserialize each object as it is encountered
            var item = serializer.Deserialize<CreateUpdateQuoteDto>(jsonReader);

            if (item == null || item.Id == 0)
                continue;  // skip invalid or unhandled entries

            // Add to batch
            batch.Add(mapper.Map<Quote>(item));
            currentBatchCount++;

            // Save in batches to avoid memory overload
            if (currentBatchCount < batchSize) continue;

            var currentBatch = new List<Quote>(batch);
            await SaveBatchAsync(currentBatch);
            batch.Clear();
            currentBatchCount = 0;
        }

        if (batch.Count > 0)
        {
            await SaveBatchAsync(batch);
        }
    }

    private async Task UpdateQuoteLengthCache(IEnumerable<Quote> currentBatch)
    {
        var tasks = currentBatch.Select(quote =>
            quotesLengthCacheService.IncreaseQuoteCountForLength(quote.TextLength)
        );
        //parallelize
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// When uploading a file, we're clearing all existing data
    /// to use the file as the new data source
    /// and avoid implementing a strategy to find and update items in both db and cache
    /// </summary>
    /// <returns></returns>
    private async Task DataCleanup()
    {
        await ClearQuotesRepo();
        await ClearQuotesCache();
    }

    private async Task ClearQuotesCache()
    {
        await quotesLengthCacheService.Clear();
    }

    private async Task ClearQuotesRepo()
    {
        await unitOfWork.CreateTransaction()
            .WithRepo(defaultRepository).Clear().CommitAsync();
    }

    private async Task SaveBatchAsync(List<Quote> items)
    {
        await unitOfWork.CreateTransaction()
            .WithBatchRepo(batchRepository)
            .AddBatch(items)
            .Ready()
            .CommitAsync();

        //forcing cache update since we're using EF batchInsert, and it does not track instances
        //await UpdateQuoteLengthCache(items);
    }

}
