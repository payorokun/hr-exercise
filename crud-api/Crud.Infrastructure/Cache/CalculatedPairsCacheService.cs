using Crud.Application.Cache;
using StackExchange.Redis;

namespace Crud.Infrastructure.Cache;

public class CalculatedPairsCacheService(IConnectionMultiplexer redisConnection) : ICalculatedPairsCacheService
{
    private const string Key = "calculated_pairs";

    public async Task<int?> GetPairCountByLength(int length)
    {
        var db = redisConnection.GetDatabase();
        
        var pairCount = await db.HashGetAsync(Key, length);
        
        return pairCount.HasValue ? (int)pairCount : null;
    }

    public async Task UpdateCacheForCalculatedPairs(int length, int count)
    {
        var db = redisConnection.GetDatabase();
        await db.HashSetAsync(Key, length, count);
    }

    public async Task Clear()
    {
        var db = redisConnection.GetDatabase();
        await db.KeyDeleteAsync(Key);
    }
}