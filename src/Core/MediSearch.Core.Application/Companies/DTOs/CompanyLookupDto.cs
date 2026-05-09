namespace MediSearch.Core.Application.Companies.DTOs;

public sealed record CompanyLookupDto
{
    public required Guid Id { get; init; }
    public required int CompanyTypeId { get; init; }
    public required string Name { get; init; }
    public required string ImageKey { get; init; }
}
