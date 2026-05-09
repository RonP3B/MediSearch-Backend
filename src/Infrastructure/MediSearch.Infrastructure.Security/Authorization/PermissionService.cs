using System.Data;
using Dapper;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Persistence.Shared.ConnectionFactories;
using Microsoft.Extensions.Caching.Hybrid;

namespace MediSearch.Infrastructure.Security.Authorization;

internal sealed class PermissionService(IDbConnectionFactory dbConnection, HybridCache hybridCache)
    : IPermissionService
{
    private readonly IDbConnectionFactory _dbConnection = dbConnection;
    private readonly HybridCache _hybridCache = hybridCache;

    private const string CacheKeyPrefix = "perms:byroles:";
    private static readonly TimeSpan CacheAbsoluteExpiration = TimeSpan.FromHours(24);
    private static readonly TimeSpan CacheLocalExpiration = TimeSpan.FromHours(4);

    public async Task<bool> IsPermissionGrantedByRolesAsync(
        IReadOnlyList<string> userRoles,
        string permissionName,
        CancellationToken cancellationToken = default
    )
    {
        if (AreUserRolesOrPermissionNameInvalid(userRoles, permissionName))
        {
            return false;
        }

        List<string> sortedRoles =
        [
            .. userRoles.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(r => r),
        ];

        string cacheKey =
            $"{CacheKeyPrefix}{string.Join("|", sortedRoles)}:{permissionName.ToLowerInvariant()}";

        return await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async ct => await CheckPermissionInDbAsync(sortedRoles, permissionName, ct),
            new HybridCacheEntryOptions
            {
                Expiration = CacheAbsoluteExpiration,
                LocalCacheExpiration = CacheLocalExpiration,
            },
            cancellationToken: cancellationToken
        );
    }

    private async Task<bool> CheckPermissionInDbAsync(
        List<string> sortedRoles,
        string permissionName,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _dbConnection.OpenConnectionAsync();

        return await conn.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1 FROM role_permissions rp
                INNER JOIN roles r ON rp.role_id = r.id
                WHERE r.name = ANY(@Roles) AND rp.permission_code = @PermissionCode
            );
            """,
            new { Roles = sortedRoles, PermissionCode = permissionName }
        );
    }

    private static bool AreUserRolesOrPermissionNameInvalid(
        IReadOnlyList<string> userRoles,
        string permissionName
    )
    {
        return userRoles is null
            || userRoles.Count == 0
            || string.IsNullOrWhiteSpace(permissionName);
    }
}
