using FluentValidation;
using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.AddProductToFavorites;

public sealed class AddProductToFavoritesCommandValidator
    : AbstractValidator<AddProductToFavoritesCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUser _currentUser;

    public AddProductToFavoritesCommandValidator(
        IProductRepository productRepository,
        ICompanyRepository companyRepository,
        ICurrentUser currentUser
    )
    {
        _productRepository = productRepository;
        _companyRepository = companyRepository;
        _currentUser = currentUser;

        RuleFor(v => v.ProductId)
            .ValidValueObject(EntityId<Product>.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v.ProductId)
                    .MustAsync(ProductExists)
                    .WithCustomErrorCode(ApplicationErrorCodes.ProductDoesNotExist)
                    .DependentRules(() =>
                        RuleFor(v => v.ProductId)
                            .MustAsync(ProductCanBeFavoritedByCurrentUser)
                            .WithCustomErrorCode(FavoriteErrorCodes.ProductNotEligibleForFavorite)
                    )
            );
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.ExistsAsync(
            EntityId<Product>.From(productId),
            cancellationToken
        );
    }

    private async Task<bool> ProductCanBeFavoritedByCurrentUser(
        Guid productId,
        CancellationToken cancellationToken
    )
    {
        CompanyType expectedType;

        // If the user is associated with a company, they can only favorite products from the opposite type of company.
        if (_currentUser.CompanyId.HasValue)
        {
            var companyType = await _companyRepository.GetCompanyTypeAsync(
                EntityId<Company>.From(_currentUser.GetAuthenticatedUserCompanyId()),
                cancellationToken
            );

            expectedType =
                companyType == CompanyType.Pharmacy ? CompanyType.Laboratory : CompanyType.Pharmacy;
        }
        // If the user is not associated with any company, it is a client user which can only favorite products from pharmacies.
        else
        {
            expectedType = CompanyType.Pharmacy;
        }

        return await _productRepository.BelongsToCompanyTypeAsync(
            EntityId<Product>.From(productId),
            expectedType,
            cancellationToken
        );
    }
}
