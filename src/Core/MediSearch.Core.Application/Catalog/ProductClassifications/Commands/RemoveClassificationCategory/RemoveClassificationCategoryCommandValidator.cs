using FluentValidation;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.RemoveClassificationCategory;

public sealed class RemoveClassificationCategoryCommandValidator
    : AbstractValidator<RemoveClassificationCategoryCommand>
{
    public RemoveClassificationCategoryCommandValidator()
    {
        RuleFor(v => v.ClassificationId).ValidValueObject(EntityId<ProductClassification>.TryFrom);

        RuleFor(v => v.CategoryId).ValidValueObject(EntityId<ClassificationCategory>.TryFrom);
    }
}
