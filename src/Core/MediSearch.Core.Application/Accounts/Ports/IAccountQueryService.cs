using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Accounts.Ports;

public interface IAccountQueryService : IQueryService
{
    Task<string?> GetExternalUserIdByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken
    );

    Task<UserContactInfoDto> GetUserContactInfoByExternalUserIdAsync(
        string externalUserId,
        CancellationToken cancellationToken
    );

    Task<UserClaimsDto?> GetUserClaimsByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken
    );

    Task<UserClaimsDto?> GetUserClaimsByUserIdOrDefaultAsync(
        Guid userId,
        CancellationToken cancellationToken
    );

    Task<UserOnboardingDto> GetUserOnboardingDataAsync(
        string externalUserId,
        CancellationToken cancellationToken
    );
}
