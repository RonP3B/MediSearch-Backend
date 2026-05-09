using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Favorites.ProductFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.AddProductToFavorites;

public sealed class AddProductToFavoritesCommandHandler(
    ICurrentUser currentUser,
    IProductFavoriteRepository productFavoriteRepository
) : ICommandHandler<AddProductToFavoritesCommand>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IProductFavoriteRepository _productFavoriteRepository =
        productFavoriteRepository;

    public async Task Handle(AddProductToFavoritesCommand cmd, CancellationToken cancellationToken)
    {
        Agent currentAgent = _currentUser.ToAgent();

        var productId = EntityId<Product>.From(cmd.ProductId);

        var existing = await _productFavoriteRepository.GetByProductAndAgentOrDefaultAsync(
            productId,
            currentAgent,
            cancellationToken
        );

        if (existing != null)
        {
            return;
        }

        _productFavoriteRepository.Add(ProductFavorite.Create(currentAgent, productId));
    }
}
