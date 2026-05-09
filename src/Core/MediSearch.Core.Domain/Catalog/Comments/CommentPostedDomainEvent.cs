namespace MediSearch.Core.Domain.Catalog.Comments;

public sealed class CommentPostedDomainEvent(
    Guid commentId,
    Guid productId,
    Guid userId,
    string content
) : DomainEvent
{
    public Guid CommentId { get; } = commentId;
    public Guid ProductId { get; } = productId;
    public Guid UserId { get; } = userId;
    public string Content { get; } = content;
}
