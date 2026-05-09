namespace MediSearch.Core.Application.Shared.DTOs;

public sealed record ItemCountDto
{
    public required string Name { get; init; }
    public required long Quantity { get; init; }
}
