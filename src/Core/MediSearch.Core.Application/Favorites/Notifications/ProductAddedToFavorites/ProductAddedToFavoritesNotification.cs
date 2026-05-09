using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Favorites.Notifications.ProductAddedToFavorites;

public sealed record ProductAddedToFavoritesNotification(
    string CompanyName,
    string ProductName,
    string FavoriterName,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
