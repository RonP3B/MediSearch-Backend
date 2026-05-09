using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;

namespace MediSearch.Core.Application.Catalog.ProductClassifications;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductClassification, ProductClassificationDto>();
        config.NewConfig<ClassificationCategory, ClassificationCategoryDto>();
    }
}
