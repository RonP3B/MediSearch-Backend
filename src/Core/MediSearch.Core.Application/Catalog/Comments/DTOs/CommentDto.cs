namespace MediSearch.Core.Application.Catalog.Comments.DTOs;

public sealed record CommentDto
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required string Content { get; init; }
    public required AuthorDto Author { get; init; }
    public Guid? ParentCommentId { get; init; } = null;
}
