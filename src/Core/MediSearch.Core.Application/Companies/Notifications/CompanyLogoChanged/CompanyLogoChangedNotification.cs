using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Companies.Notifications.CompanyLogoChanged;

public sealed record CompanyLogoChangedNotification(
    string CompanyName,
    string NewImageKey,
    IReadOnlyList<UserContactInfoDto> FavoritersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
