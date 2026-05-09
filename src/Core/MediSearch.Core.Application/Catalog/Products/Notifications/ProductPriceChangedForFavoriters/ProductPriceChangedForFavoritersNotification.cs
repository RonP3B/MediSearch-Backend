using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForFavoriters;

public sealed record ProductPriceChangedForFavoritersNotification(
    string ProductName,
    string PreviousPrice,
    string NewPrice,
    IReadOnlyList<UserContactInfoDto> FavoritersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
