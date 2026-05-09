using FluentValidation;
using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Extensions;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IProductClassificationRepository _productClassificationRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateProductCommandValidator(
        IProductClassificationRepository productClassificationRepository,
        IProductRepository productRepository,
        ICurrentUser currentUser
    )
    {
        _productClassificationRepository = productClassificationRepository;
        _productRepository = productRepository;
        _currentUser = currentUser;

        RuleFor(v => v.Name)
            .ValidValueObject(ProductName.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v)
                    .UniqueProductNameInCompany(
                        _productRepository,
                        _currentUser,
                        nameSelector: cmd => cmd.Name,
                        excludeIdSelector: cmd => cmd.Id
                    )
                    .OverridePropertyName(nameof(UpdateProductCommand.Name))
            );

        RuleFor(v => v.Description).ValidValueObject(CleanText.TryFrom);

        RuleFor(v => v)
            .ValidValueObject(
                cmd => Price.TryFrom(cmd.PriceAmount, cmd.PriceCurrency),
                prefix: ProductPrefixes.Price
            );

        RuleFor(v => v.Quantity).ValidValueObject(Quantity.TryFrom);

        // Optional validation that runs only if ImageFiles is not null and contains at least one file
        RuleForEach(v => v.ImageFiles)
            .ChildRules(iv =>
                iv.RuleFor(file => file)
                    .NotEmpty()
                    .DependentRules(() => iv.RuleFor(file => file).ImageFile().WithMaxSizeMB(5))
            );

        RuleFor(v => v.ClassificationId)
            .ValidValueObject(EntityId<ProductClassification>.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v.ClassificationId)
                    .ProductClassificationExists(_productClassificationRepository)
            );

        RuleFor(v => v.CategoryIds)
            .NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithCustomErrorCode(ProductErrorCodes.DuplicateCategoryIds)
            .DependentRules(() =>
            {
                RuleForEach(v => v.CategoryIds)
                    .ValidValueObject(EntityId<ClassificationCategory>.TryFrom);

                RuleFor(v => v.CategoryIds)
                    .CategoriesBelongToClassification(
                        _productClassificationRepository,
                        cmd => cmd.ClassificationId
                    );
            })
            .When(v => EntityId<ProductClassification>.TryFrom(v.ClassificationId).IsSuccess);
    }
}
