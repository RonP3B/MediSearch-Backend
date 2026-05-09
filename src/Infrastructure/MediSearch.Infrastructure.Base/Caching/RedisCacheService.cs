using System.Buffers;
using System.Text.Json;
using MediSearch.Core.Application.Shared.Ports;
using StackExchange.Redis;

namespace MediSearch.Infrastructure.Base.Caching;

internal sealed class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private IDatabase Db => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await Db.StringGetAsync(key);
        return value.IsNull ? default : Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var bytes = Serialize(value);

        await Db.StringSetAsync(
            key,
            bytes,
            expiration.HasValue ? (Expiration)expiration.Value : Expiration.Default
        );
    }

    public async Task<bool> SetIfNotExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var bytes = Serialize(value);

        return await Db.StringSetAsync(key, bytes, expiration, When.NotExists);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        await Db.KeyDeleteAsync(key);

    public async Task RemoveManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default
    ) => await Db.KeyDeleteAsync([.. keys.Select(k => (RedisKey)k)]);

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize cached value of type {typeof(T).Name}."
            );
    }
}
