namespace MediSearch.Core.Application.Catalog.Comments.DTOs;

public sealed record AuthorCompanyDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
