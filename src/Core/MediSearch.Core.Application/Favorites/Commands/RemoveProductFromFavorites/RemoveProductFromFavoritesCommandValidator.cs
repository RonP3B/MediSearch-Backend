using FluentValidation;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.RemoveProductFromFavorites;

public sealed class RemoveProductFromFavoritesCommandValidator
    : AbstractValidator<RemoveProductFromFavoritesCommand>
{
    public RemoveProductFromFavoritesCommandValidator()
    {
        RuleFor(v => v.ProductId).ValidValueObject(EntityId<Product>.TryFrom);
    }
}
