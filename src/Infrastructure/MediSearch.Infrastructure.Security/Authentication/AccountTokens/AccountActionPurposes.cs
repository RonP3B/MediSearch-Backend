namespace MediSearch.Infrastructure.Security.Authentication.AccountTokens;

/// <summary>
/// Purposes an account action token can be issued for. The purpose is part of the signed
/// payload, so an email confirmation link can never be replayed as a password reset link.
/// </summary>
internal static class AccountActionPurposes
{
    public const string EmailConfirmation = "email-confirmation";
    public const string PasswordReset = "password-reset";
}
