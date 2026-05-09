namespace MediSearch.Core.Application.Accounts.Notifications.AccountEmailConfirmed;

public sealed record AccountEmailConfirmedNotification(string ExternalUserId, string IdempotencyKey)
    : Notification(IdempotencyKey);
