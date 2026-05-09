namespace MediSearch.Core.Application.Catalog.Comments.DTOs;

public sealed record CommentReplyForAuthorDto
{
    public required Guid ParentCommentAuthorUserId { get; init; }
    public required string ParentCommentAuthorEmail { get; init; }
    public required string ParentCommentAuthorFullName { get; init; }
    public required string ParentCommentAuthorUsername { get; init; }
    public required string ReplyAuthorName { get; init; }

    public string ParentCommentAuthorDisplayName =>
        $"{ParentCommentAuthorFullName} ({ParentCommentAuthorUsername})";
}
