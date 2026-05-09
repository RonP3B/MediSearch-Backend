namespace MediSearch.Core.Application.Accounts.Notifications.EmailConfirmationRequested;

public sealed record EmailConfirmationRequestedNotification(
    string ExternalUserId,
    string ConfirmationToken,
    string? IdempotencyKey = null
) : Notification(IdempotencyKey);
