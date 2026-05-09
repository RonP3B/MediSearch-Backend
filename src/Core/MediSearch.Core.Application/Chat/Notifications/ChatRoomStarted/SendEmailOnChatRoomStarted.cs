using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Application.Chat.Models;

namespace MediSearch.Core.Application.Chat.Notifications.ChatRoomStarted;

public sealed class SendEmailOnChatRoomStarted(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ChatRoomStartedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ChatRoomStartedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ChatRoomStartedModel(
                CompanyName: notification.CompanyName,
                CounterpartyName: notification.CounterpartyName
            ),
            cancellationToken
        );

        var messages = notification.CompanyUsersContactInfo.Select(contactInfo => new EmailMessage(
            contactInfo.Email,
            ChatEmailSubjectCodes.ChatRoomStartedEmailSubject,
            body,
            contactInfo.DisplayName
        ));

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
