namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record RegisteredExternalUserDto
{
    public required string Id { get; init; }
}
