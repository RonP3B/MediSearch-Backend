using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Domain.Catalog.Comments;

public sealed class Comment : BaseAuditableEntity
{
    private Comment() { }

    public EntityId<Comment> Id { get; private set; } = EntityId<Comment>.Empty;
    public EntityId<User> UserId { get; private set; } = EntityId<User>.Empty;
    public EntityId<Product> ProductId { get; private set; } = EntityId<Product>.Empty;
    public CleanText Content { get; private set; } = CleanText.Empty;
    public EntityId<Comment>? ParentCommentId { get; private set; }

    public bool IsReply => ParentCommentId is not null;

    public static Comment Create(
        CleanText content,
        EntityId<User> userId,
        EntityId<Product> productId
    )
    {
        var commentId = EntityId<Comment>.New();

        var comment = new Comment
        {
            Id = commentId,
            Content = content,
            UserId = userId,
            ProductId = productId,
        };

        comment.AddDomainEvent(
            new CommentPostedDomainEvent(
                comment.Id,
                comment.ProductId,
                comment.UserId,
                comment.Content
            )
        );

        return comment;
    }

    public static Comment CreateCommentReply(
        CleanText content,
        EntityId<User> userId,
        Comment parent
    )
    {
        if (parent.IsReply)
        {
            throw new BusinessRuleException(nameof(IsReply), CommentErrorCodes.CannotReplyToReply);
        }

        var commentId = EntityId<Comment>.New();

        var commentReply = new Comment
        {
            Id = commentId,
            Content = content,
            UserId = userId,
            ParentCommentId = parent.Id,
            ProductId = parent.ProductId,
        };

        commentReply.AddDomainEvent(
            new CommentRepliedToDomainEvent(
                commentReply.ParentCommentId,
                commentReply.Id,
                commentReply.UserId,
                commentReply.Content
            )
        );

        return commentReply;
    }

    public void EditContent(CleanText newContent, DateTimeOffset now)
    {
        DateTimeOffset oneHourAfterCreation = CreatedAt.AddHours(1);

        if (now > oneHourAfterCreation)
        {
            throw new BusinessRuleException(nameof(Content), CommentErrorCodes.EditWindowExpired);
        }

        Content = newContent;
    }
}
