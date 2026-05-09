using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Companies.Notifications.CompanyRenamed;

public sealed record CompanyRenamedNotification(
    string PreviousCompanyName,
    string NewCompanyName,
    IReadOnlyList<UserContactInfoDto> FavoritersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
