using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductOutOfStock;

public sealed record ProductOutOfStockNotification(
    string ProductName,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
