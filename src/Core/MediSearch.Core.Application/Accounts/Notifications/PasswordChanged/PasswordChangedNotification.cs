namespace MediSearch.Core.Application.Accounts.Notifications.PasswordChanged;

public sealed record PasswordChangedNotification(
    string ExternalUserId,
    string? IdempotencyKey = null
) : Notification(IdempotencyKey);
