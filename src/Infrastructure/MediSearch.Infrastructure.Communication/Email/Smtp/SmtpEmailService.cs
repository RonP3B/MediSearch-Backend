using MailKit.Net.Smtp;
using MailKit.Security;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Application.Shared.Types;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MediSearch.Infrastructure.Communication.Email.Smtp;

internal sealed class SmtpEmailService(
    ICacheService cacheService,
    ITextLocalizer textLocalizer,
    IOptions<SmtpOptions> options,
    ILogger<SmtpEmailService> logger
) : IEmailService
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly ITextLocalizer _textLocalizer = textLocalizer;
    private readonly IOptions<SmtpOptions> _options = options;
    private readonly ILogger<SmtpEmailService> _logger = logger;

    private const string CacheKeyPrefix = "email-sent:";
    private const string FromTestingName = "MediSearch Dev";
    private const string FromTestingEmail = "noreply@medisearch.com";

    private static readonly TimeSpan IdempotencyExpiration = TimeSpan.FromHours(24);

    public int MaxBatchSize => int.MaxValue;

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
            var message = BuildMessage(emailMessage);
            await SendWithConnectionAsync(message, cancellationToken);
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
            foreach (var message in messages)
            {
                var mimeMessage = BuildMessage(message);
                await SendWithConnectionAsync(mimeMessage, cancellationToken);
            }
        }
        catch
        {
            await _cacheService.RemoveAsync($"{CacheKeyPrefix}{idempotencyKey}", cancellationToken);
            throw;
        }
    }

    private async Task SendWithConnectionAsync(
        MimeMessage message,
        CancellationToken cancellationToken
    )
    {
        using SmtpClient smtpClient = new();

        var settings = _options.Value;

        await smtpClient.ConnectAsync(
            settings.Host,
            settings.Port,
            SecureSocketOptions.None, // MailPit = no TLS
            cancellationToken
        );

        try
        {
            await smtpClient.SendAsync(message, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "[Email Failure] Unable to send email "
                    + "| Recipient: '{Recipient}' "
                    + "| Subject: '{Subject}' | SMTP: {SmtpHost}:{Port}",
                message.To,
                message.Subject,
                settings.Host,
                settings.Port
            );

            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }

    private MimeMessage BuildMessage(EmailMessage emailMessage)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(FromTestingName, FromTestingEmail));
        message.To.Add(new MailboxAddress(emailMessage.ToDisplayName, emailMessage.ToEmailAddress));
        message.Subject = _textLocalizer[emailMessage.Subject];
        message.Body = new TextPart("html") { Text = emailMessage.Body };

        return message;
    }
}
