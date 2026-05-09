namespace MediSearch.Core.Application.Shared.DTOs;

public sealed record EnumerationDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
