using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Favorites.ProductFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.RemoveProductFromFavorites;

public sealed class RemoveProductFromFavoritesCommandHandler(
    ICurrentUser currentUser,
    IProductFavoriteRepository productFavoriteRepository
) : ICommandHandler<RemoveProductFromFavoritesCommand>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IProductFavoriteRepository _productFavoriteRepository =
        productFavoriteRepository;

    public async Task Handle(
        RemoveProductFromFavoritesCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        var productFavorite = await _productFavoriteRepository.GetByProductAndAgentOrDefaultAsync(
            EntityId<Product>.From(cmd.ProductId),
            currentAgent,
            cancellationToken
        );

        if (productFavorite == null)
        {
            return;
        }

        _productFavoriteRepository.Remove(productFavorite);
    }
}
