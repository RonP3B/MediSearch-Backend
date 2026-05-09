using FluentValidation;
using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Extensions;

internal static class ValidationExtensions
{
    /// <summary>
    /// Validates that a product name is unique within the current user's company.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productRepository">The product repository to check uniqueness.</param>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, string> UniqueProductNameInCompany<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IProductRepository productRepository,
        ICurrentUser currentUser
    )
    {
        return ruleBuilder
            .MustAsync(
                async (name, cancellationToken) =>
                    !await productRepository.ExistsWithNameAsync(
                        EntityId<Company>.From(currentUser.GetAuthenticatedUserCompanyId()),
                        ProductName.From(name),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(ProductErrorCodes.ProductNameAlreadyExists);
    }

    /// <summary>
    /// Validates that a product name is unique within the current user's company,
    /// excluding the specified product. Used in update scenarios to allow a product
    /// to retain its current name.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productRepository">The product repository to check uniqueness.</param>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <param name="nameSelector">A function to extract the product name from the request.</param>
    /// <param name="excludeIdSelector">A function to extract the product ID to exclude from the uniqueness check.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, T}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, T> UniqueProductNameInCompany<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        IProductRepository productRepository,
        ICurrentUser currentUser,
        Func<T, string> nameSelector,
        Func<T, Guid> excludeIdSelector
    )
    {
        return ruleBuilder
            .MustAsync(
                async (cmd, _, cancellationToken) =>
                {
                    var id = EntityId<Product>.TryFrom(excludeIdSelector(cmd));

                    if (!id.IsSuccess)
                        return false;

                    return !await productRepository.ExistsWithNameAsync(
                        EntityId<Company>.From(currentUser.GetAuthenticatedUserCompanyId()),
                        ProductName.From(nameSelector(cmd)),
                        id.Value,
                        cancellationToken
                    );
                }
            )
            .WithCustomErrorCode(ProductErrorCodes.ProductNameAlreadyExists);
    }

    /// <summary>
    /// Validates that a product classification exists in the system.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productClassificationRepository">The product classification repository to check existence.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, Guid}"/> configured with async existence validation.
    /// </returns>
    public static IRuleBuilderOptions<T, Guid> ProductClassificationExists<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IProductClassificationRepository productClassificationRepository
    )
    {
        return ruleBuilder
            .MustAsync(
                async (productClassificationId, cancellationToken) =>
                    await productClassificationRepository.ExistsAsync(
                        EntityId<ProductClassification>.From(productClassificationId),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(ProductClassificationErrorCodes.ProductClassificationDoesNotExist);
    }

    /// <summary>
    /// Validates that all specified product categories belong to the given product classification.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productClassificationRepository">The product classification repository to check category membership.</param>
    /// <param name="classificationIdSelector">A function to extract the classification ID from the request.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, IEnumerable{Guid}}"/> configured with async category membership validation.
    /// </returns>
    public static IRuleBuilderOptions<T, IEnumerable<Guid>> CategoriesBelongToClassification<T>(
        this IRuleBuilder<T, IEnumerable<Guid>> ruleBuilder,
        IProductClassificationRepository productClassificationRepository,
        Func<T, Guid> classificationIdSelector
    )
    {
        return ruleBuilder
            .MustAsync(
                async (cmd, categoryIds, cancellationToken) =>
                {
                    var categoryEntityIds = categoryIds
                        .Select(EntityId<ClassificationCategory>.From)
                        .ToList();

                    return await productClassificationRepository.CategoriesBelongToClassificationAsync(
                        EntityId<ProductClassification>.From(classificationIdSelector(cmd)),
                        categoryEntityIds,
                        cancellationToken
                    );
                }
            )
            .WithCustomErrorCode(ProductClassificationErrorCodes.CategoryNotInClassification);
    }
}
