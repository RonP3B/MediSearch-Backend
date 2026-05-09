namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record RefreshedAccessTokenDto
{
    public required string AccessToken { get; init; }
}
