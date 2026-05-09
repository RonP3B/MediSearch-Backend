using MediSearch.Core.Application.Catalog.Comments.Constants;
using MediSearch.Core.Application.Catalog.Comments.Models;

namespace MediSearch.Core.Application.Catalog.Comments.Notifications.CommentPosted;

public sealed class SendEmailOnCommentPosted(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CommentPostedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CommentPostedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CommentPostedModel(
                CompanyName: notification.CompanyName,
                ProductName: notification.ProductName,
                CommentAuthorName: notification.CommentAuthorName,
                CommentContent: notification.CommentContent
            ),
            cancellationToken
        );

        var messages = notification.CompanyUsersContactInfo.Select(contactInfo => new EmailMessage(
            contactInfo.Email,
            CommentEmailSubjectCodes.CommentPostedEmailSubject,
            body,
            contactInfo.DisplayName
        ));

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
