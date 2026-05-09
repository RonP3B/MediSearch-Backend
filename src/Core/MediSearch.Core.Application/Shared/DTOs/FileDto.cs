namespace MediSearch.Core.Application.Shared.DTOs;

public sealed record FileDto
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required Stream Content { get; init; }
}
