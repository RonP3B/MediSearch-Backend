namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record AuthenticationTokensDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
