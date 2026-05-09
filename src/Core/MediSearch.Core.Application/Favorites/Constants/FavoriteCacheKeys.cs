namespace MediSearch.Core.Application.Favorites.Constants;

internal static class FavoriteCacheKeys
{
    public static string CompanyAddedToFavoritesNotification(Guid favoriterAgentId, Guid companyId) =>
        $"notify-favorite:{favoriterAgentId}:{companyId}";

    public static string ProductAddedToFavoritesNotification(Guid favoriterAgentId, Guid productId) =>
        $"notify-favorite:{favoriterAgentId}:{productId}";
}
