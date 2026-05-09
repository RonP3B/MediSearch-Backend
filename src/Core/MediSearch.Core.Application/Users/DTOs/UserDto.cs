namespace MediSearch.Core.Application.Users.DTOs;

public sealed record UserDto
{
    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required IReadOnlyList<EnumerationDto> Roles { get; init; }
    public string? ProfileImageKey { get; init; } = null;
    public Guid? CompanyId { get; init; } = null;
}
