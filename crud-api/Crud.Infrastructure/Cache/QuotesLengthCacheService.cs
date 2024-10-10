using Crud.Application.Cache;
using StackExchange.Redis;

namespace Crud.Infrastructure.Cache;
public class QuotesLengthCacheService(IConnectionMultiplexer redisConnection) : IQuotesLengthCacheService
{
    private const string Key = "quotes_by_text_length";
    public async Task IncreaseQuoteCountForLength(int length)
    {
        var db = redisConnection.GetDatabase();
        await db.SortedSetIncrementAsync(Key, length.ToString(), 1);
    }

    public async Task DecreaseQuoteCountForLength(int length)
    {
        var db = redisConnection.GetDatabase();
        await db.SortedSetIncrementAsync(Key, length.ToString(), -1);

        // If the count reaches 0, remove the length from the sorted set
        var count = await db.SortedSetScoreAsync(Key, length.ToString());
        if (count <= 0)
        {
            await db.SortedSetRemoveAsync(Key, length.ToString());
        }
    }

    public async Task<List<QuotesLengthCacheEntity>> GetByTextLength(int length)
    {
        var db = redisConnection.GetDatabase();
        var quoteLengths = await db.SortedSetRangeByScoreWithScoresAsync(Key, 0, length);
        return quoteLengths.Select(q=> new QuotesLengthCacheEntity((int)q.Score, (int)q.Element)).ToList();
    }
}