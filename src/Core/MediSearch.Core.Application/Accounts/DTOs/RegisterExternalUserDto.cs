namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record RegisterExternalUserDto
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public bool IsActive { get; init; } = false;
}
