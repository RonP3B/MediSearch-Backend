using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<ProductDto, ProductDetailsDto>()
            .Ignore(dest => dest.Categories)
            .Ignore(dest => dest.Classification)
            .Ignore(dest => dest.Company)
            .Ignore(dest => dest.Comments);
    }
}
