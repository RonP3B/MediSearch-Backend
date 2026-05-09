namespace MediSearch.Core.Application.Users.DTOs;

public sealed record UserContactInfoDto
{
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string Username { get; init; }
    public string DisplayName => $"{FullName} ({Username})";
}
