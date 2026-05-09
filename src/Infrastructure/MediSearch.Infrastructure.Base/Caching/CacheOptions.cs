using Microsoft.Extensions.Caching.Distributed;

namespace MediSearch.Infrastructure.Base.Caching;

public static class CacheOptions
{
    public static readonly DistributedCacheEntryOptions DefaultExpiration = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : DefaultExpiration;
}
