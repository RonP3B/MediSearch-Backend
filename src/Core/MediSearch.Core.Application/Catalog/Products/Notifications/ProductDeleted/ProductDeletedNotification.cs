using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductDeleted;

public sealed record ProductDeletedNotification(
    string ProductName,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
