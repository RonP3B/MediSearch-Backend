using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Accounts.Queries;

internal sealed class AccountQueryService(IDbConnectionFactory db) : IAccountQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<string?> GetExternalUserIdByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<string>(
            AccountSql.GetExternalUserIdByUsername,
            new { Username = username.ToLowerInvariant() }
        );
    }

    public async Task<UserClaimsDto?> GetUserClaimsByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<UserClaimsDto>(
            AccountSql.GetUserClaimsByUsername,
            new { Username = username.ToLowerInvariant() }
        );
    }

    public async Task<UserClaimsDto?> GetUserClaimsByUserIdOrDefaultAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<UserClaimsDto>(
            AccountSql.GetUserClaimsByUserId,
            new { UserId = userId }
        );
    }

    public async Task<UserContactInfoDto> GetUserContactInfoByExternalUserIdAsync(
        string externalUserId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<UserContactInfoDto>(
            AccountSql.GetUserContactInfoByExternalUserId,
            new { ExternalUserId = externalUserId }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Contact info for external user '{externalUserId}' was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<UserOnboardingDto> GetUserOnboardingDataAsync(
        string externalUserId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<UserOnboardingDto>(
            AccountSql.GetUserOnboardingData,
            new { ExternalUserId = externalUserId }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Onboarding data for external user '{externalUserId}' was expected to exist but was not found."
            );
        }

        return result;
    }
}
