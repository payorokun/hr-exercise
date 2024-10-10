using Microsoft.EntityFrameworkCore.Diagnostics;
using Crud.Application.Cache;
using Crud.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Crud.Infrastructure.UnitOfWork;
public class QuoteChangeInterceptor(IQuotesLengthCacheService quotesLengthCacheService, ICalculatedPairsCacheService calculatedPairsCacheService) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        await ClearCalculatedPairsCache();
        
        var entries = context.ChangeTracker.Entries<Quote>();

        await UpdateCacheWithChanges(entries);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task UpdateCacheWithChanges(IEnumerable<EntityEntry<Quote>> entries)
    {
        foreach (var entry in entries)
        {
            var quoteLength = entry.Entity.TextLength;

            switch (entry.State)
            {
                case EntityState.Added:
                    // A new quote is being created, so increase the Redis counter
                    await quotesLengthCacheService.IncreaseQuoteCountForLength(quoteLength);
                    break;

                case EntityState.Modified:
                    // Check if the text length changed before deciding
                    var originalLength = entry.OriginalValues.GetValue<int>(nameof(entry.Entity.TextLength));
                    if (originalLength != quoteLength)
                    {
                        // Decrease the count for the original length
                        await quotesLengthCacheService.DecreaseQuoteCountForLength(originalLength);
                        // Increase the count for the new length
                        await quotesLengthCacheService.IncreaseQuoteCountForLength(quoteLength);
                    }
                    break;

                case EntityState.Deleted:
                    // A quote is being deleted, so decrease the Redis counter
                    await quotesLengthCacheService.DecreaseQuoteCountForLength(quoteLength);
                    break;
            }
        }
    }

    private async Task ClearCalculatedPairsCache()
    {
        await calculatedPairsCacheService.Clear();
    }
}

