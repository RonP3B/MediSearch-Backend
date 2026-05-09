namespace MediSearch.Core.Application.Companies.DTOs;

public record CompanyDto
{
    public required Guid Id { get; init; }
    public required int CompanyTypeId { get; init; }
    public required string Name { get; init; }
    public required string CeoName { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required string ImageKey { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public string? Website { get; init; } = null;
    public string? Facebook { get; init; } = null;
    public string? Instagram { get; init; } = null;
    public string? Twitter { get; init; } = null;
}
