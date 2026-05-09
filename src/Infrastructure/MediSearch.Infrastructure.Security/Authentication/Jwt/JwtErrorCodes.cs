namespace MediSearch.Infrastructure.Security.Authentication.Jwt;

internal static class JwtErrorCodes
{
    public const string ExpiredRefreshToken = "Infrastructure.Shared.ExpiredRefreshToken";
    public const string InvalidTokenSignature = "Infrastructure.Shared.InvalidTokenSignature";
    public const string TokenValidationFailed = "Infrastructure.Shared.TokenValidationFailed";
    public const string InvalidRefreshToken = "Infrastructure.Shared.InvalidRefreshToken";
}
