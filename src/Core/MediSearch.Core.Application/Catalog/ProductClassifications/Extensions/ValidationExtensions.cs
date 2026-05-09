using FluentValidation;
using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Extensions;

internal static class ValidationExtensions
{
    /// <summary>
    /// Validates that a product classification name is unique.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productClassificationRepository">The repository to check name uniqueness.</param>
    /// <returns>A rule builder options instance for further configuration.</returns>
    public static IRuleBuilderOptions<T, string> UniqueProductClassificationName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IProductClassificationRepository productClassificationRepository
    )
    {
        return ruleBuilder
            .MustAsync(
                async (name, cancellationToken) =>
                    !await productClassificationRepository.ExistsByNameAsync(
                        Name.From(name),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(ProductClassificationErrorCodes.ClassificationAlreadyExists);
    }

    /// <summary>
    /// Validates that a product classification name is unique,
    /// excluding the specified classification. Used in update scenarios
    /// to allow a classification to retain its current name.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="productClassificationRepository">The repository to check name uniqueness.</param>
    /// <param name="nameSelector">A function to extract the classification name from the request.</param>
    /// <param name="excludeIdSelector">A function to extract the classification ID to exclude from the uniqueness check.</param>
    /// <returns>A rule builder options instance for further configuration.</returns>
    public static IRuleBuilderOptions<T, T> UniqueProductClassificationName<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        IProductClassificationRepository productClassificationRepository,
        Func<T, string> nameSelector,
        Func<T, Guid> excludeIdSelector
    )
    {
        return ruleBuilder
            .MustAsync(
                async (cmd, _, cancellationToken) =>
                {
                    var name = Name.TryFrom(nameSelector(cmd));
                    var id = EntityId<ProductClassification>.TryFrom(excludeIdSelector(cmd));

                    if (!name.IsSuccess || !id.IsSuccess)
                        return false;

                    return !await productClassificationRepository.ExistsByNameAsync(
                        name.Value,
                        id.Value,
                        cancellationToken
                    );
                }
            )
            .WithCustomErrorCode(ProductClassificationErrorCodes.ClassificationAlreadyExists);
    }
}
