namespace MediSearch.Infrastructure.Security.Authentication.Jwt;

public sealed record JwtOptions
{
    public required string AccessTokenSecretKey { get; init; }
    public required string RefreshTokenSecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessTokenExpirationMinutes { get; init; }
    public required int RefreshTokenExpirationDays { get; init; }
}
