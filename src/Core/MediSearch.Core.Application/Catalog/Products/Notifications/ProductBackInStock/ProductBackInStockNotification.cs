using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductBackInStock;

public sealed record ProductBackInStockNotification(
    string ProductName,
    IReadOnlyList<UserContactInfoDto> FavoritersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
