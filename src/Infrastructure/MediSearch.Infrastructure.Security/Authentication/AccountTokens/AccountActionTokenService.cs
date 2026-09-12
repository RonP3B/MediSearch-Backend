using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Security.Authentication.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediSearch.Infrastructure.Security.Authentication.AccountTokens;

/// <summary>
/// Issues and validates the single-purpose tokens that travel inside account emails:
/// the email confirmation link and the password reset link.
/// </summary>
/// <remarks>
/// These used to come from ASP.NET Core Identity's token providers. Keycloak can send its
/// own action emails, but it never returns the token to the caller, which would mean giving
/// up the application's Razor templates, localization and MailPit/Resend pipeline. So the
/// API signs them itself: a small JWT carrying the account id, the purpose and (for password
/// resets) a stamp taken from Keycloak, all signed with a dedicated secret.
/// </remarks>
internal sealed class AccountActionTokenService(
    IOptions<AccountTokenOptions> accountTokenOptions,
    IOptions<JwtOptions> jwtOptions,
    IDateTimeProvider dateTimeProvider
)
{
    private const string PurposeClaimType = "purpose";
    private const string StampClaimType = "stamp";

    private static readonly JwtSecurityTokenHandler TokenHandler = new()
    {
        MapInboundClaims = false,
    };

    private readonly AccountTokenOptions _accountTokenOptions = accountTokenOptions.Value;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public string CreateEmailConfirmationToken(string externalUserId)
    {
        return Create(
            AccountActionPurposes.EmailConfirmation,
            externalUserId,
            stamp: null,
            TimeSpan.FromHours(_accountTokenOptions.EmailConfirmationTokenLifetimeHours)
        );
    }

    public string CreatePasswordResetToken(string externalUserId, string stamp)
    {
        return Create(
            AccountActionPurposes.PasswordReset,
            externalUserId,
            stamp,
            TimeSpan.FromHours(_accountTokenOptions.PasswordResetTokenLifetimeHours)
        );
    }

    public bool IsEmailConfirmationTokenValid(string token, string externalUserId)
    {
        return IsValid(
            token,
            AccountActionPurposes.EmailConfirmation,
            externalUserId,
            expectedStamp: null
        );
    }

    public bool IsPasswordResetTokenValid(string token, string externalUserId, string stamp)
    {
        return IsValid(token, AccountActionPurposes.PasswordReset, externalUserId, stamp);
    }

    private string Create(string purpose, string externalUserId, string? stamp, TimeSpan lifetime)
    {
        SigningCredentials signingCredentials = new(CreateSigningKey(), SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, externalUserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(PurposeClaimType, purpose),
        ];

        if (!string.IsNullOrEmpty(stamp))
        {
            claims.Add(new Claim(StampClaimType, stamp));
        }

        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow.Add(lifetime),
            signingCredentials: signingCredentials
        );

        return TokenHandler.WriteToken(token);
    }

    private bool IsValid(
        string token,
        string purpose,
        string externalUserId,
        string? expectedStamp
    )
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = CreateSigningKey(),
            ClockSkew = TimeSpan.Zero,
        };

        try
        {
            ClaimsPrincipal principal = TokenHandler.ValidateToken(
                token,
                validationParameters,
                out SecurityToken validatedToken
            );

            if (
                validatedToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return false;
            }

            return HasClaimValue(principal, JwtRegisteredClaimNames.Sub, externalUserId)
                && HasClaimValue(principal, PurposeClaimType, purpose)
                && HasExpectedStamp(principal, expectedStamp);
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private SymmetricSecurityKey CreateSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accountTokenOptions.SecretKey));
    }

    private static bool HasClaimValue(ClaimsPrincipal principal, string claimType, string expected)
    {
        string? value = principal.FindFirst(claimType)?.Value;

        return string.Equals(value, expected, StringComparison.Ordinal);
    }

    private static bool HasExpectedStamp(ClaimsPrincipal principal, string? expectedStamp)
    {
        if (expectedStamp is null)
        {
            return true;
        }

        string? value = principal.FindFirst(StampClaimType)?.Value;

        return string.Equals(value ?? string.Empty, expectedStamp, StringComparison.Ordinal);
    }
}
