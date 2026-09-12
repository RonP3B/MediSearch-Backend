namespace MediSearch.Infrastructure.Security.Authentication.AccountTokens;

/// <summary>
/// Settings for the short-lived tokens the API puts inside account emails.
/// </summary>
/// <remarks>
/// ASP.NET Core Identity used to generate these tokens through its token providers.
/// Keycloak has no equivalent API that hands a token back to the caller, so MediSearch
/// signs its own. They are separate from the login tokens in <c>JwtOptions</c> and use
/// their own secret, so leaking one never affects the other.
/// </remarks>
public sealed record AccountTokenOptions
{
    /// <summary>
    /// HMAC-SHA256 signing key. Must be at least 32 characters.
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// How long an email confirmation link stays valid. Defaults to 24 hours, the same
    /// lifetime ASP.NET Core Identity used.
    /// </summary>
    public int EmailConfirmationTokenLifetimeHours { get; init; } = 24;

    /// <summary>
    /// How long a password reset link stays valid.
    /// </summary>
    public int PasswordResetTokenLifetimeHours { get; init; } = 2;
}
