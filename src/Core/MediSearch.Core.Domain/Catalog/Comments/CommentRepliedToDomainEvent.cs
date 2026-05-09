namespace MediSearch.Core.Domain.Catalog.Comments;

public sealed class CommentRepliedToDomainEvent(
    Guid parentCommentId,
    Guid commentId,
    Guid userId,
    string content
) : DomainEvent
{
    public Guid ParentCommentId { get; } = parentCommentId;
    public Guid CommentId { get; } = commentId;
    public Guid UserId { get; } = userId;
    public string Content { get; } = content;
}
