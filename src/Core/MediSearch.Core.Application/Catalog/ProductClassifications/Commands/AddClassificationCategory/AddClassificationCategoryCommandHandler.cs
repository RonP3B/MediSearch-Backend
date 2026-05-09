using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.AddClassificationCategory;

public sealed class AddClassificationCategoryCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<AddClassificationCategoryCommand, ClassificationCategoryDto>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public async Task<ClassificationCategoryDto> Handle(
        AddClassificationCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var productClassification = await _productClassificationRepository.GetByIdOrDefaultAsync(
            EntityId<ProductClassification>.From(cmd.ClassificationId),
            cancellationToken
        );

        if (productClassification is null)
        {
            throw NotFoundException.Entity(
                nameof(ProductClassification),
                nameof(ProductClassification.Id),
                cmd.ClassificationId
            );
        }

        var category = ClassificationCategory.Create(
            productClassification.Id,
            Name.From(cmd.CategoryName)
        );

        productClassification.AddCategory(category);

        return category.Adapt<ClassificationCategoryDto>();
    }
}
