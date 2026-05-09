using System.Security.Claims;
using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Helpers;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler(
    IAuthenticationTokenService tokenService,
    IAccountQueryService accountQueryService,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<RefreshAccessTokenCommand, RefreshedAccessTokenDto>
{
    private readonly IAuthenticationTokenService _tokenService = tokenService;
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<RefreshedAccessTokenDto> Handle(
        RefreshAccessTokenCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var result = await _tokenService.ValidateRefreshTokenAsync(cmd.RefreshToken);

        if (!result.Succeeded)
        {
            var errorCode = result.Errors.SelectMany(e => e.Value).First();
            throw new UnauthorizedException(errorCode);
        }

        IEnumerable<Claim> refreshTokenClaims = await _tokenService.ReadTokenClaimsAsync(
            cmd.RefreshToken
        );

        string? userIdClaim = refreshTokenClaims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException(AccountErrorCodes.InvalidRefreshTokenPayload);
        }

        var userClaims = await _accountQueryService.GetUserClaimsByUserIdOrDefaultAsync(
            userId,
            cancellationToken
        );

        if (userClaims is null)
        {
            throw new UnauthorizedException(AccountErrorCodes.AccountDoesNotExist);
        }

        return new RefreshedAccessTokenDto
        {
            AccessToken = await _tokenService.GenerateAccessTokenAsync(
                AccessTokenClaimsBuilder.BuildAccessTokenClaims(
                    userClaims,
                    _dateTimeProvider.UtcNow
                )
            ),
        };
    }
}
