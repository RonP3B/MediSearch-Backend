using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Helpers;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Commands.Login;

public sealed class LoginCommandHandler(
    IAuthenticationTokenService tokenService,
    ICredentialsValidator credentialsValidator,
    IDateTimeProvider dateTimeProvider,
    IAccountQueryService accountQueryService
) : ICommandHandler<LoginCommand, AuthenticationTokensDto>
{
    private readonly IAuthenticationTokenService _tokenService = tokenService;
    private readonly ICredentialsValidator _credentialsValidator = credentialsValidator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAccountQueryService _accountQueryService = accountQueryService;

    public async Task<AuthenticationTokensDto> Handle(
        LoginCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var user = await _accountQueryService.GetUserClaimsByUsernameOrDefaultAsync(
            cmd.Username,
            cancellationToken
        );

        if (user is null)
        {
            throw new UnauthorizedException(AccountErrorCodes.AccountNotRegistered);
        }

        var result = await _credentialsValidator.ValidateCredentialsAsync(
            cmd.Username,
            cmd.Password,
            cancellationToken
        );

        if (!result.Succeeded)
        {
            var errorCode = result.Errors.SelectMany(e => e.Value).First();
            throw new UnauthorizedException(errorCode);
        }

        return new AuthenticationTokensDto
        {
            RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id.ToString()),
            AccessToken = await _tokenService.GenerateAccessTokenAsync(
                AccessTokenClaimsBuilder.BuildAccessTokenClaims(user, _dateTimeProvider.UtcNow)
            ),
        };
    }
}
