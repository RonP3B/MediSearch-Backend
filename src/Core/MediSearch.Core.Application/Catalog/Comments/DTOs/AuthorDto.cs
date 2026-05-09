namespace MediSearch.Core.Application.Catalog.Comments.DTOs;

public sealed record AuthorDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string ProfileImageKey { get; init; }
    public AuthorCompanyDto? Company { get; init; } = null;
}
