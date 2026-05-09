namespace MediSearch.Core.Application.Companies.DTOs;

public sealed record RegisterCompanyOwnerDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required FileDto ProfileImageFile { get; init; }
}
