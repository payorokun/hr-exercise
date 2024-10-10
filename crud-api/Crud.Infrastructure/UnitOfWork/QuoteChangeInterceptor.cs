using Microsoft.EntityFrameworkCore.Diagnostics;
using Crud.Application.Cache;
using Crud.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Crud.Infrastructure.UnitOfWork;
public class QuoteChangeInterceptor(IQuotesLengthCacheService quotesLengthCacheService) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        // Get all tracked entities (quotes in this case) and detect their states
        var entries = context.ChangeTracker.Entries<Quote>();

        foreach (var entry in entries)
        {
            var quoteLength = entry.Entity.Text.Length;

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

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}

