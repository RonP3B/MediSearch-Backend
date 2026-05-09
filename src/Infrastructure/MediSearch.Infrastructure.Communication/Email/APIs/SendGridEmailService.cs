using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Application.Shared.Types;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MediSearch.Infrastructure.Communication.Email.APIs;

internal sealed class SendGridEmailService(
    ITextLocalizer textLocalizer,
    ICacheService cacheService,
    IOptionsSnapshot<EmailServiceOptions> options,
    ILogger<SendGridEmailService> logger
) : IEmailService
{
    private readonly ITextLocalizer _textLocalizer = textLocalizer;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsSnapshot<EmailServiceOptions> _options = options;
    private readonly ILogger<SendGridEmailService> _logger = logger;

    private const string CacheKeyPrefix = "email-sent:";
    private static readonly TimeSpan IdempotencyExpiration = TimeSpan.FromHours(24);

    public int MaxBatchSize => 1000;

    public async Task SendAsync(
        EmailMessage emailMessage,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    )
    {
        bool isNew = await _cacheService.SetIfNotExistsAsync(
            $"{CacheKeyPrefix}{idempotencyKey}",
            true,
            IdempotencyExpiration,
            cancellationToken
        );

        if (!isNew)
        {
            return;
        }

        try
        {
            var settings = _options.Value;
            var client = new SendGridClient(settings.ApiKey);
            var message = BuildMessage(emailMessage, settings);
            var response = await client.SendEmailAsync(message, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "[Email Failure] Unable to send email "
                        + "| Recipient: '{Recipient}' | Address: '{Address}' "
                        + "| Subject: '{Subject}' | Status: {StatusCode} | Response: {Response} | Provider: SendGrid",
                    emailMessage.ToDisplayName,
                    emailMessage.ToEmailAddress,
                    emailMessage.Subject,
                    response.StatusCode,
                    body
                );

                throw new InvalidOperationException(
                    $"SendGrid rejected the email send request with status code {(int)response.StatusCode}."
                );
            }
        }
        catch
        {
            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{idempotencyKey}", cancellationToken);
            throw;
        }
    }

    public async Task SendBulkAsync(
        IEnumerable<EmailMessage> messages,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    )
    {
        var messageList = messages.ToList();

        if (messageList.Count > MaxBatchSize)
        {
            throw new InvalidOperationException(
                $"SendGrid batch limit is {MaxBatchSize} recipients per request. Got {messageList.Count}."
            );
        }

        bool isNew = await _cacheService.SetIfNotExistsAsync(
            $"{CacheKeyPrefix}{idempotencyKey}",
            true,
            IdempotencyExpiration,
            cancellationToken
        );

        if (!isNew)
        {
            return;
        }

        try
        {
            var settings = _options.Value;
            var client = new SendGridClient(settings.ApiKey);
            EnsureBulkMessagesShareContent(messageList);

            var sendGridMessage = BuildBulkMessage(messageList, settings);

            var response = await client.SendEmailAsync(sendGridMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "[Email Failure] Unable to send bulk email "
                        + "| Recipients: {Count} | Status: {StatusCode} | Response: {Response} | Provider: SendGrid",
                    messageList.Count,
                    response.StatusCode,
                    body
                );

                throw new InvalidOperationException(
                    $"SendGrid rejected the bulk email send request with status code {(int)response.StatusCode}."
                );
            }
        }
        catch
        {
            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{idempotencyKey}", cancellationToken);
            throw;
        }
    }

    private SendGridMessage BuildMessage(EmailMessage emailMessage, EmailServiceOptions settings) =>
        MailHelper.CreateSingleEmail(
            from: new EmailAddress(settings.FromEmail, settings.FromName),
            to: new EmailAddress(emailMessage.ToEmailAddress, emailMessage.ToDisplayName),
            subject: _textLocalizer[emailMessage.Subject],
            plainTextContent: null,
            htmlContent: emailMessage.Body
        );

    private static void EnsureBulkMessagesShareContent(IReadOnlyList<EmailMessage> messages)
    {
        var firstMessage = messages.First();

        if (
            messages.Any(message =>
                message.Subject != firstMessage.Subject || message.Body != firstMessage.Body
            )
        )
        {
            throw new InvalidOperationException(
                "SendGrid bulk sending requires every email in the batch to share the same subject and body."
            );
        }
    }

    private SendGridMessage BuildBulkMessage(
        IReadOnlyList<EmailMessage> messages,
        EmailServiceOptions settings
    )
    {
        var firstMessage = messages.First();

        return new SendGridMessage
        {
            From = new EmailAddress(settings.FromEmail, settings.FromName),
            Subject = _textLocalizer[firstMessage.Subject],
            HtmlContent = firstMessage.Body,
            Personalizations =
            [
                .. messages.Select(message => new Personalization
                {
                    Tos = [new EmailAddress(message.ToEmailAddress, message.ToDisplayName)],
                }),
            ],
        };
    }
}
