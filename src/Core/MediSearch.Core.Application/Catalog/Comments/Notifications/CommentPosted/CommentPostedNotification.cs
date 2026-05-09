using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Notifications.CommentPosted;

public sealed record CommentPostedNotification(
    string CompanyName,
    string ProductName,
    string CommentAuthorName,
    string CommentContent,
    IReadOnlyList<UserContactInfoDto> CompanyUsersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
