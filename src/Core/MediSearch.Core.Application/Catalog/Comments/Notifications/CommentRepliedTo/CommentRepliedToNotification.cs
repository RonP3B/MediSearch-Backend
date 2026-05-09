namespace MediSearch.Core.Application.Catalog.Comments.Notifications.CommentRepliedTo;

public sealed record CommentRepliedToNotification(
    string ParentCommentAuthorEmail,
    string ParentCommentAuthorDisplayName,
    string ReplyAuthorName,
    string ReplyContent,
    string IdempotencyKey
) : Notification(IdempotencyKey);
