namespace MediSearch.Core.Application.Accounts.Notifications.PasswordResetSuccess;

public sealed record PasswordResetSuccessNotification(
    string ExternalUserId,
    string? IdempotencyKey = null
) : Notification(IdempotencyKey);
