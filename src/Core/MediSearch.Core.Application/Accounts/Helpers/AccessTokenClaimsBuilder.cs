using System.Security.Claims;
using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MediSearch.Core.Application.Accounts.Helpers;

internal static class AccessTokenClaimsBuilder
{
    /// <summary>
    /// Builds a collection of claims for an access token based on user information.
    /// </summary>
    /// <param name="user">The user claims data containing identity and role information.</param>
    /// <returns>
    /// A read-only collection of <see cref="Claim"/> objects including identity, company, and role claims.
    /// </returns>
    public static IReadOnlyCollection<Claim> BuildAccessTokenClaims(
        UserClaimsDto user,
        DateTimeOffset issuedAtUtc
    )
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(CustomClaimTypes.ExternalUserId, user.ExternalId),
            new(
                JwtRegisteredClaimNames.Iat,
                issuedAtUtc.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
        };

        if (user.CompanyId is Guid companyId)
        {
            claims.Add(new Claim(CustomClaimTypes.CompanyId, companyId.ToString()));
        }

        foreach (string role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims.AsReadOnly();
    }
}
