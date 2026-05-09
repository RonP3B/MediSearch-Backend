namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record UserClaimsDto
{
    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
    public Guid? CompanyId { get; init; } = null;
}
