using Crud.Application.Cache;
using StackExchange.Redis;

namespace Crud.Infrastructure.Cache;
public class QuotesLengthCacheService(IConnectionMultiplexer redisConnection) : IQuotesLengthCacheService
{
    private const string Key = "quotes_by_text_length";
    public async Task IncreaseQuoteCountForLength(int length)
    {
        var db = redisConnection.GetDatabase();
        await db.HashIncrementAsync(Key, length.ToString(), 1);
    }

    public async Task DecreaseQuoteCountForLength(int length)
    {
        var db = redisConnection.GetDatabase();
        await db.HashIncrementAsync(Key, length.ToString(), -1);

        // If the count reaches 0, remove the length from the sorted set
        var count = await db.HashGetAsync(Key, length.ToString());
        if (count.HasValue || (int)count <= 0)
        {
            await db.HashDeleteAsync(Key, length.ToString());
        }
    }

    public async Task<List<QuotesLengthCacheEntity>> GetByTextLength(int length)
    {
        var db = redisConnection.GetDatabase();
        var hashEntries = await db.HashGetAllAsync(Key);
        var quoteLengths = hashEntries
            .Select(entry => new QuotesLengthCacheEntity(int.Parse(entry.Name), (int) entry.Value))
            .Where(q => q.Length <= length)
            .OrderBy(q => q.Length)
            .ToList();
        return quoteLengths;
    }

    public Task Clear()
    {
        var db = redisConnection.GetDatabase();
        return db.KeyDeleteAsync(Key);
    }
}