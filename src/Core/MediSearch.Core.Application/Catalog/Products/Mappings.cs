using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Domain.Catalog.Products;

namespace MediSearch.Core.Application.Catalog.Products;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Product, ProductDto>()
            .Map(dest => dest.PriceAmount, src => src.Price.Amount)
            .Map(dest => dest.PriceCurrency, src => src.Price.Currency);
    }
}
