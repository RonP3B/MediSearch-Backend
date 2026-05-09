using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateClassificationCategory;

public sealed class UpdateClassificationCategoryCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<UpdateClassificationCategoryCommand, ClassificationCategoryDto>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public async Task<ClassificationCategoryDto> Handle(
        UpdateClassificationCategoryCommand cmd,
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

        var categoryId = EntityId<ClassificationCategory>.From(cmd.CategoryId);

        productClassification.UpdateCategoryName(categoryId, Name.From(cmd.CategoryName));

        var updatedCategory = productClassification.Categories.First(c => c.Id == categoryId);

        return updatedCategory.Adapt<ClassificationCategoryDto>();
    }
}
