using AutoMapper;
using Crud.Application.Cache;
using Crud.Application.Repositories;
using Crud.Domain.Models;
using MediatR;

namespace Crud.Application.Quotes.PairCount;
public class GetQuotePairCountByLengthQueryHandler(IRepository<Quote> repository, 
    IQuotesLengthCacheService quotesLengthCache,
    ICalculatedPairsCacheService calculatedPairsCacheService) : IRequestHandler<GetQuotePairCountByLengthQuery, QuotePairCountQueryResult>
{
    public async Task<QuotePairCountQueryResult> Handle(GetQuotePairCountByLengthQuery request, CancellationToken cancellationToken)
    {
        var quotePairCountQueryResult = await CountPairs(request.MaxLength);
        //cache the count for future usages
        await UpdateCacheForCalculatedPairs(request.MaxLength, quotePairCountQueryResult.Count);
        return quotePairCountQueryResult;
    }

    private async Task UpdateCacheForCalculatedPairs(int maxLength, int newCount)
    {
        await calculatedPairsCacheService.UpdateCacheForCalculatedPairs(maxLength, newCount);
    }

    private async Task<QuotePairCountQueryResult> CountPairs(int maxLength)
    {
        var alreadyCalculatedPairs = await calculatedPairsCacheService.GetPairCountByLength(maxLength);
        if (alreadyCalculatedPairs.HasValue)
        {
            return new QuotePairCountQueryResult(maxLength, alreadyCalculatedPairs.Value);
        }

        // Retrieve all keys (quote lengths) and their corresponding counts from Redis
        var quoteLengths = await quotesLengthCache.GetByTextLength(maxLength);

        // Two-pointer approach to find the number of pairs
        var totalPairs = 0;
        
        var left = 0;
        var right = quoteLengths.Count - 1;

        while (left <= right)
        {
            var leftLength = quoteLengths[left].Length;
            var leftCount = quoteLengths[left].Count;

            var rightLength = quoteLengths[right].Length;
            var rightCount = quoteLengths[right].Count;

            if (leftLength + rightLength <= maxLength)
            {
                if (left == right)
                {
                    // Pairs within the same length, use combination formula
                    totalPairs += (leftCount * (leftCount - 1)) / 2;
                }
                else
                {
                    // Pairs between different lengths, just a*b
                    totalPairs += leftCount * rightCount;
                }
                left++;
            }
            else
            {
                right--;
            }
        }

        return new QuotePairCountQueryResult(maxLength, totalPairs);
    }
}
