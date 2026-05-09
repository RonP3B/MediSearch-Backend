using FluentValidation;
using MediSearch.Core.Application.Catalog.ProductClassifications.Extensions;
using MediSearch.Core.Domain.Catalog.ProductClassifications;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.CreateProductClassification;

public sealed class CreateProductClassificationCommandValidator
    : AbstractValidator<CreateProductClassificationCommand>
{
    private readonly IProductClassificationRepository _productClassificationRepository;

    public CreateProductClassificationCommandValidator(
        IProductClassificationRepository productClassificationRepository
    )
    {
        _productClassificationRepository = productClassificationRepository;

        RuleFor(v => v.Name)
            .ValidValueObject(Name.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v.Name)
                    .UniqueProductClassificationName(_productClassificationRepository)
            );
    }
}
