using FluentValidation;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.RemoveCompanyFromFavorites;

public sealed class RemoveCompanyFromFavoritesCommandValidator
    : AbstractValidator<RemoveCompanyFromFavoritesCommand>
{
    public RemoveCompanyFromFavoritesCommandValidator()
    {
        RuleFor(v => v.CompanyId).ValidValueObject(EntityId<Company>.TryFrom);
    }
}
