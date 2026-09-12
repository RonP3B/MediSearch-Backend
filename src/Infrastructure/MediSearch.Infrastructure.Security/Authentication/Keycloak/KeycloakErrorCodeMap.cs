using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Translates Keycloak failures into the application's account error codes, so the API
/// keeps returning exactly the same codes it returned with ASP.NET Core Identity.
/// </summary>
/// <remarks>
/// Keycloak is not as consistent as <c>IdentityResult</c>: some endpoints answer with a
/// message key such as <c>invalidPasswordMinLengthMessage</c>, others with a plain English
/// sentence such as <c>User exists with same username</c>. Both shapes are handled, keys
/// first and free text as a fallback.
/// </remarks>
internal static class KeycloakErrorCodeMap
{
    private static readonly IReadOnlyDictionary<string, string> ByMessageKey = new Dictionary<
        string,
        string
    >(StringComparer.OrdinalIgnoreCase)
    {
        ["invalidPasswordMinLengthMessage"] = AccountErrorCodes.PasswordTooShort,
        ["invalidPasswordMinUpperCaseCharsMessage"] = AccountErrorCodes.PasswordRequiresUpper,
        ["invalidPasswordMinLowerCaseCharsMessage"] = AccountErrorCodes.PasswordRequiresLower,
        ["invalidPasswordMinDigitsMessage"] = AccountErrorCodes.PasswordRequiresDigit,
        ["invalidPasswordMinSpecialCharsMessage"] =
            AccountErrorCodes.PasswordRequiresNonAlphanumeric,
        ["invalidPasswordMinUniqueCharsMessage"] = AccountErrorCodes.PasswordRequiresUniqueChars,
        ["invalidEmailMessage"] = AccountErrorCodes.InvalidEmail,
        ["missingEmailMessage"] = AccountErrorCodes.InvalidEmail,
        ["invalidUsernameMessage"] = AccountErrorCodes.InvalidUsername,
        ["missingUsernameMessage"] = AccountErrorCodes.InvalidUsername,
        ["usernameExistsMessage"] = AccountErrorCodes.DuplicateUsername,
        ["emailExistsMessage"] = AccountErrorCodes.DuplicateEmail,
    };

    private static readonly (string Fragment, string ErrorCode)[] ByMessageFragment =
    [
        ("user exists with same username", AccountErrorCodes.DuplicateUsername),
        ("username already exists", AccountErrorCodes.DuplicateUsername),
        ("user exists with same email", AccountErrorCodes.DuplicateEmail),
        ("email already exists", AccountErrorCodes.DuplicateEmail),
        ("minimum length", AccountErrorCodes.PasswordTooShort),
        ("upper case", AccountErrorCodes.PasswordRequiresUpper),
        ("lower case", AccountErrorCodes.PasswordRequiresLower),
        ("special characters", AccountErrorCodes.PasswordRequiresNonAlphanumeric),
        ("numerical digits", AccountErrorCodes.PasswordRequiresDigit),
        ("invalid email", AccountErrorCodes.InvalidEmail),
        ("invalid username", AccountErrorCodes.InvalidUsername),
    ];

    public static string Resolve(KeycloakErrorResponse? error)
    {
        if (error is null)
        {
            return AccountErrorCodes.UnknownAccountError;
        }

        if (error.Key is not null && ByMessageKey.TryGetValue(error.Key, out string? mappedByKey))
        {
            return mappedByKey;
        }

        string? message = error.Message;

        if (string.IsNullOrWhiteSpace(message))
        {
            return AccountErrorCodes.UnknownAccountError;
        }

        foreach ((string fragment, string errorCode) in ByMessageFragment)
        {
            if (message.Contains(fragment, StringComparison.OrdinalIgnoreCase))
            {
                return errorCode;
            }
        }

        return AccountErrorCodes.UnknownAccountError;
    }
}
