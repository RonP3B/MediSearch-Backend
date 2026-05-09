using FluentValidation;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateClassificationCategory;

public sealed class UpdateClassificationCategoryCommandValidator
    : AbstractValidator<UpdateClassificationCategoryCommand>
{
    public UpdateClassificationCategoryCommandValidator()
    {
        RuleFor(v => v.ClassificationId).ValidValueObject(EntityId<ProductClassification>.TryFrom);

        RuleFor(v => v.CategoryId).ValidValueObject(EntityId<ClassificationCategory>.TryFrom);

        RuleFor(v => v.CategoryName).ValidValueObject(Name.TryFrom);
    }
}
