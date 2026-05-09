namespace MediSearch.Core.Application.Accounts.Constants;

public static class AccountErrorCodes
{
    public const string UnknownAccountError = "Application.Accounts.UnknownAccountError";
    public const string InvalidRefreshTokenPayload =
        "Application.Accounts.InvalidRefreshTokenPayload";
    public const string AccountNotRegistered = "Application.Accounts.AccountNotRegistered";
    public const string AccountDoesNotExist = "Application.Accounts.AccountDoesNotExist";
    public const string InvalidCredentials = "Application.Accounts.InvalidCredentials";
    public const string AccountEmailNotConfirmed = "Application.Accounts.AccountEmailNotConfirmed";
    public const string InvalidToken = "Application.Accounts.InvalidToken";
    public const string AccountEmailAlreadyConfirmed =
        "Application.Accounts.AccountEmailAlreadyConfirmed";
    public const string DuplicateEmail = "Application.Accounts.DuplicateEmail";
    public const string DuplicateUsername = "Application.Accounts.DuplicateUsername";
    public const string InvalidEmail = "Application.Accounts.InvalidEmail";
    public const string InvalidUsername = "Application.Accounts.InvalidUsername";
    public const string PasswordTooShort = "Application.Accounts.PasswordTooShort";
    public const string PasswordRequiresUpper = "Application.Accounts.PasswordRequiresUpper";
    public const string PasswordRequiresLower = "Application.Accounts.PasswordRequiresLower";
    public const string PasswordRequiresDigit = "Application.Accounts.PasswordRequiresDigit";
    public const string PasswordMismatch = "Application.Accounts.PasswordMismatch";
    public const string PasswordRequiresNonAlphanumeric =
        "Application.Accounts.PasswordRequiresNonAlphanumeric";
    public const string PasswordRequiresUniqueChars =
        "Application.Accounts.PasswordRequiresUniqueChars";
    public const string NewPasswordMustNotMatchOldOne =
        "Application.Accounts.NewPasswordMustNotMatchOldOne";
}
