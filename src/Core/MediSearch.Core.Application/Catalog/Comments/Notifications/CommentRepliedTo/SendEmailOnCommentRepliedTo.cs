using MediSearch.Core.Application.Catalog.Comments.Constants;
using MediSearch.Core.Application.Catalog.Comments.Models;

namespace MediSearch.Core.Application.Catalog.Comments.Notifications.CommentRepliedTo;

public sealed class SendEmailOnCommentRepliedTo(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CommentRepliedToNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CommentRepliedToNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CommentRepliedToModel(
                ReplyAuthorName: notification.ReplyAuthorName,
                ReplyContent: notification.ReplyContent
            ),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                notification.ParentCommentAuthorEmail,
                CommentEmailSubjectCodes.CommentRepliedToEmailSubject,
                body,
                notification.ParentCommentAuthorDisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
