using FluentValidation;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.DeleteProductClassification;

public sealed class DeleteProductClassificationCommandValidator
    : AbstractValidator<DeleteProductClassificationCommand>
{
    public DeleteProductClassificationCommandValidator()
    {
        RuleFor(v => v.Id).ValidValueObject(EntityId<ProductClassification>.TryFrom);
    }
}
