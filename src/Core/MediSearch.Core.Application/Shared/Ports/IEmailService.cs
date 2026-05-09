namespace MediSearch.Core.Application.Shared.Ports;

public interface IEmailService
{
    int MaxBatchSize { get; }

    Task SendAsync(
        EmailMessage message,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    );

    Task SendBulkAsync(
        IEnumerable<EmailMessage> messages,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    );
}
