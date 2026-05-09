namespace MediSearch.Core.Application.Accounts.Notifications.PasswordResetRequested;

public sealed record PasswordResetRequestedNotification(
    string ExternalUserId,
    string ResetToken,
    string? IdempotencyKey = null
) : Notification(IdempotencyKey);
