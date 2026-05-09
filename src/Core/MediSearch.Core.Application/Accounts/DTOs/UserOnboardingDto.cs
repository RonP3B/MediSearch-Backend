namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record UserOnboardingDto
{
    public required string FullName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required int[] RoleIds { get; init; }
    public string? CompanyName { get; init; } = null;
    public string DisplayName => $"{FullName} ({Username})";
}
