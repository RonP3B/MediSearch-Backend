namespace MediSearch.Core.Application.Users.Notifications.CompanyUserRemoved;

public sealed record CompanyUserRemovedNotification(
    string Email,
    string FullName,
    string Username,
    string CompanyName,
    string IdempotencyKey
) : Notification(IdempotencyKey);
