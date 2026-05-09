using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Favorites.DTOs;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Favorites.Ports;

public interface IFavoriteQueryService : IQueryService
{
    Task<CompanyFavoritedByDto> GetCompanyFavoritedByAsync(
        Guid companyId,
        Guid favoriterAgentId,
        int favoriterAgentTypeId,
        CancellationToken cancellationToken
    );

    Task<ProductFavoritedByDto> GetProductFavoritedByAsync(
        Guid productId,
        Guid favoriterAgentId,
        int favoriterAgentTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<UserContactInfoDto>> GetCompanyFavoritersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<UserContactInfoDto>> GetProductFavoritersContactInfoAsync(
        Guid productId,
        CancellationToken cancellationToken
    );

    Task<FavoritesDto> GetAllFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<CompanySummaryDto>> GetCompanyFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<ProductPreviewDto>> GetProductFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    );
}
