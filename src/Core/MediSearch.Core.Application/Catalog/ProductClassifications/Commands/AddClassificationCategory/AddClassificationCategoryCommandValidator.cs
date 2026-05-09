using FluentValidation;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.AddClassificationCategory;

public sealed class AddClassificationCategoryCommandValidator
    : AbstractValidator<AddClassificationCategoryCommand>
{
    public AddClassificationCategoryCommandValidator()
    {
        RuleFor(v => v.ClassificationId).ValidValueObject(EntityId<ProductClassification>.TryFrom);

        RuleFor(v => v.CategoryName).ValidValueObject(Name.TryFrom);
    }
}
