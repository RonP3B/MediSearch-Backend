using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using ApplicationEmail = MediSearch.Core.Application.Shared.Types.EmailMessage;
using ResendEmail = Resend.EmailMessage;

namespace MediSearch.Infrastructure.Communication.Email.APIs;

internal sealed class ResendEmailService(
    IResend resendClient,
    ITextLocalizer textLocalizer,
    IOptionsSnapshot<EmailServiceOptions> options,
    ILogger<ResendEmailService> logger
) : IEmailService
{
    private readonly IResend _resendClient = resendClient;
    private readonly ITextLocalizer _textLocalizer = textLocalizer;
    private readonly IOptionsSnapshot<EmailServiceOptions> _options = options;
    private readonly ILogger<ResendEmailService> _logger = logger;

    public int MaxBatchSize => 100;

    public async Task SendAsync(
        ApplicationEmail emailMessage,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _resendClient.EmailSendAsync(
                idempotencyKey,
                BuildMessage(emailMessage),
                cancellationToken
            );
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "[Email Failure] Unable to send email "
                    + "| Recipient: '{Recipient}' | Address: '{Address}' "
                    + "| Subject: '{Subject}' | Provider: Resend",
                emailMessage.ToDisplayName,
                emailMessage.ToEmailAddress,
                emailMessage.Subject
            );

            throw;
        }
    }

    public async Task SendBulkAsync(
        IEnumerable<ApplicationEmail> messages,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    )
    {
        var resendMessages = messages.Select(BuildMessage).ToList();

        if (resendMessages.Count > MaxBatchSize)
        {
            throw new InvalidOperationException(
                $"Resend batch limit is {MaxBatchSize} messages per request. Got {resendMessages.Count}."
            );
        }

        try
        {
            await _resendClient.EmailBatchAsync(idempotencyKey, resendMessages, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "[Email Failure] Unable to send bulk email "
                    + "| Recipients: {Count} | Provider: Resend",
                messages.Count()
            );

            throw;
        }
    }

    private ResendEmail BuildMessage(ApplicationEmail emailMessage)
    {
        var settings = _options.Value;

        return new ResendEmail
        {
            From = $"{settings.FromName} <{settings.FromEmail}>",
            To = [emailMessage.ToEmailAddress],
            Subject = _textLocalizer[emailMessage.Subject],
            HtmlBody = emailMessage.Body,
        };
    }
}
