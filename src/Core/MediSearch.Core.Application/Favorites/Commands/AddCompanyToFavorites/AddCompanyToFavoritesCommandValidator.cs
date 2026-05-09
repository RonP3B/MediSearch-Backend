using FluentValidation;
using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.AddCompanyToFavorites;

public sealed class AddCompanyToFavoritesCommandValidator
    : AbstractValidator<AddCompanyToFavoritesCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUser _currentUser;

    public AddCompanyToFavoritesCommandValidator(
        ICompanyRepository companyRepository,
        ICurrentUser currentUser
    )
    {
        _companyRepository = companyRepository;
        _currentUser = currentUser;

        RuleFor(v => v.CompanyId)
            .ValidValueObject(EntityId<Company>.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v.CompanyId)
                    .MustAsync(CompanyExists)
                    .WithCustomErrorCode(ApplicationErrorCodes.CompanyDoesNotExist)
                    .DependentRules(() =>
                        RuleFor(v => v.CompanyId)
                            .MustAsync(CompanyCanBeFavoritedByCurrentUser)
                            .WithCustomErrorCode(FavoriteErrorCodes.CompanyNotEligibleForFavorite)
                    )
            );
    }

    private async Task<bool> CompanyExists(Guid companyId, CancellationToken cancellationToken)
    {
        return await _companyRepository.ExistsAsync(
            EntityId<Company>.From(companyId),
            cancellationToken
        );
    }

    private async Task<bool> CompanyCanBeFavoritedByCurrentUser(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        // Client user (no company): only pharmacies allowed
        if (!_currentUser.CompanyId.HasValue)
        {
            var targetCompanyType = await _companyRepository.GetCompanyTypeAsync(
                EntityId<Company>.From(companyId),
                cancellationToken
            );

            return targetCompanyType.Id == CompanyType.Pharmacy.Id;
        }

        // Company user: can only favorite companies of a different type
        return await _companyRepository.AreCompaniesOfDifferentTypeAsync(
            EntityId<Company>.From(companyId),
            EntityId<Company>.From(_currentUser.GetAuthenticatedUserCompanyId()),
            cancellationToken
        );
    }
}
