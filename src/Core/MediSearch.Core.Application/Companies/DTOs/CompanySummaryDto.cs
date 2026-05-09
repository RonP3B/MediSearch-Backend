namespace MediSearch.Core.Application.Companies.DTOs;

public sealed record CompanySummaryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageKey { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public bool? IsFavoritedByCurrentUser { get; init; } = null;
}
