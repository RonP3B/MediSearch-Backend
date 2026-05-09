using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Favorites.Notifications.CompanyAddedToFavorites;

public sealed record CompanyAddedToFavoritesNotification(
    string CompanyName,
    string FavoriterName,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
