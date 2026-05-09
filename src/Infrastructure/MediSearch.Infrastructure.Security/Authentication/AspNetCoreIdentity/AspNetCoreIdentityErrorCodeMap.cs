using MediSearch.Core.Application.Accounts.Constants;

namespace MediSearch.Infrastructure.Security.Authentication.AspNetCoreIdentity;

internal static class AspNetCoreIdentityErrorCodeMap
{
    public static readonly IReadOnlyDictionary<string, string> Map = new Dictionary<string, string>
    {
        ["InvalidToken"] = AccountErrorCodes.InvalidToken,
        ["DuplicateEmail"] = AccountErrorCodes.DuplicateEmail,
        ["DuplicateUserName"] = AccountErrorCodes.DuplicateUsername,
        ["InvalidEmail"] = AccountErrorCodes.InvalidEmail,
        ["InvalidUserName"] = AccountErrorCodes.InvalidUsername,
        ["PasswordTooShort"] = AccountErrorCodes.PasswordTooShort,
        ["PasswordRequiresUpper"] = AccountErrorCodes.PasswordRequiresUpper,
        ["PasswordRequiresLower"] = AccountErrorCodes.PasswordRequiresLower,
        ["PasswordRequiresDigit"] = AccountErrorCodes.PasswordRequiresDigit,
        ["PasswordMismatch"] = AccountErrorCodes.PasswordMismatch,
        ["PasswordRequiresNonAlphanumeric"] = AccountErrorCodes.PasswordRequiresNonAlphanumeric,
        ["PasswordRequiresUniqueChars"] = AccountErrorCodes.PasswordRequiresUniqueChars,
    };
}
