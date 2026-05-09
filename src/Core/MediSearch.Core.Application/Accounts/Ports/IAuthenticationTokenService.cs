using System.Security.Claims;

namespace MediSearch.Core.Application.Accounts.Ports;

public interface IAuthenticationTokenService
{
    Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims);
    Task<string> GenerateRefreshTokenAsync(string userIdentifier);
    Task<ServiceResult> ValidateRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<Claim>> ReadTokenClaimsAsync(string token);
}
