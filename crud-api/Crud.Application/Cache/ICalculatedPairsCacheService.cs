namespace Crud.Application.Cache;

public interface ICalculatedPairsCacheService
{
    Task<int?> GetPairCountByLength(int length);
    Task UpdateCacheForCalculatedPairs(int length, int count);
}