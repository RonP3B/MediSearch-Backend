using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Application.Shared.Results;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediSearch.Infrastructure.Security.Authentication.Jwt;

internal sealed class AuthenticationJwtService(
    IOptions<JwtOptions> jwtOptions,
    IDateTimeProvider dateTimeProvider
) : IAuthenticationTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.AccessTokenSecretKey));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken accessToken = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(accessToken));
    }

    public Task<string> GenerateRefreshTokenAsync(string userIdentifier)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.RefreshTokenSecretKey));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [new(ClaimTypes.NameIdentifier, userIdentifier)];

        JwtSecurityToken refreshToken = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: _dateTimeProvider.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            signingCredentials: creds,
            claims: claims
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(refreshToken));
    }

    public Task<ServiceResult> ValidateRefreshTokenAsync(string refreshToken)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.RefreshTokenSecretKey));

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero,
        };

        JwtSecurityTokenHandler tokenHandler = new();

        try
        {
            ClaimsPrincipal validatedToken = tokenHandler.ValidateToken(
                refreshToken,
                tokenValidationParameters,
                out var tokenInfo
            );

            bool isValidRefreshToken = validatedToken != null && IsValidJwtSecurityToken(tokenInfo);

            if (isValidRefreshToken)
            {
                return Task.FromResult(ServiceResult.Success());
            }
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(
                ServiceResult.Failure("RefreshToken", JwtErrorCodes.ExpiredRefreshToken)
            );
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return Task.FromResult(
                ServiceResult.Failure("RefreshToken", JwtErrorCodes.InvalidTokenSignature)
            );
        }
        catch (SecurityTokenValidationException)
        {
            return Task.FromResult(
                ServiceResult.Failure("RefreshToken", JwtErrorCodes.TokenValidationFailed)
            );
        }

        return Task.FromResult(
            ServiceResult.Failure("RefreshToken", JwtErrorCodes.InvalidRefreshToken)
        );
    }

    public Task<IEnumerable<Claim>> ReadTokenClaimsAsync(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        if (!tokenHandler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid JWT token format.");
        }

        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        return Task.FromResult(jwtToken.Claims);
    }

    private static bool IsValidJwtSecurityToken(SecurityToken token)
    {
        if (token is not JwtSecurityToken jwtSecurityToken)
        {
            return false;
        }

        string algorithm = jwtSecurityToken.Header.Alg;

        bool isExpectedAlgorithm = algorithm.Equals(
            SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase
        );

        return isExpectedAlgorithm;
    }
}
