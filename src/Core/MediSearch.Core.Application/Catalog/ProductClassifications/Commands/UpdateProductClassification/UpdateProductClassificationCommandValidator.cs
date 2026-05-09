using FluentValidation;
using MediSearch.Core.Application.Catalog.ProductClassifications.Extensions;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateProductClassification;

public sealed class UpdateProductClassificationCommandValidator
    : AbstractValidator<UpdateProductClassificationCommand>
{
    private readonly IProductClassificationRepository _productClassificationRepository;

    public UpdateProductClassificationCommandValidator(
        IProductClassificationRepository productClassificationRepository
    )
    {
        _productClassificationRepository = productClassificationRepository;

        RuleFor(v => v.Id).ValidValueObject(EntityId<ProductClassification>.TryFrom);

        RuleFor(v => v.Name).ValidValueObject(Name.TryFrom);

        RuleFor(v => v)
            .UniqueProductClassificationName(
                _productClassificationRepository,
                nameSelector: cmd => cmd.Name,
                excludeIdSelector: cmd => cmd.Id
            )
            .OverridePropertyName(nameof(UpdateProductClassificationCommand.Name));
    }
}
