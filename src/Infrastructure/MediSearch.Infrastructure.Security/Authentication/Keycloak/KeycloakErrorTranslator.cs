using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Infrastructure.Security.Authentication.Constants;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Turns a failed Admin REST API call into the <c>(ErrorKey, ErrorCode)</c> pairs a
/// <c>ServiceResult</c> carries, mirroring what the ASP.NET Core Identity adapter did
/// with <c>IdentityResult.Errors</c>.
/// </summary>
internal static class KeycloakErrorTranslator
{
    public static IEnumerable<(string ErrorKey, ErrorCode ErrorCode)> Translate(
        KeycloakAdminResponse response
    )
    {
        string errorCode = KeycloakErrorCodeMap.Resolve(response.Error);

        return [(ResolveErrorKey(errorCode), new ErrorCode(errorCode))];
    }

    private static string ResolveErrorKey(string errorCode)
    {
        return errorCode switch
        {
            string code when code.Contains("Token", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Token,

            string code when code.Contains("Password", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Password,

            string code when code.Contains("Email", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Email,

            string code when code.Contains("Username", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Username,

            _ => AuthenticationErrorKeys.Account,
        };
    }
}
