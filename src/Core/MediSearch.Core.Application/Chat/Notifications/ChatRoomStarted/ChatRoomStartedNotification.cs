using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Chat.Notifications.ChatRoomStarted;

public sealed record ChatRoomStartedNotification(
    string CompanyName,
    string CounterpartyName,
    IReadOnlyList<UserContactInfoDto> CompanyUsersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
