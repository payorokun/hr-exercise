namespace Crud.Application.Cache;
public interface IQuotesLengthCacheService
{
    Task IncreaseQuoteCountForLength(int length);
    Task DecreaseQuoteCountForLength(int length);
    Task<List<QuotesLengthCacheEntity>> GetByTextLength(int length);
}