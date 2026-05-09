using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductCreated;

public sealed record ProductCreatedNotification(
    string ProductName,
    string Price,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
