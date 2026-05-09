using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Base.Caching;
using MediSearch.Shared.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private static void AddCachingServices(this IHostApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache(ServiceNames.Redis);

        builder.Services.AddHybridCache();

        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    }
}
