using MediSearch.Core.Application.Catalog.Products.Commands.CreateProduct;
using MediSearch.Core.Application.Catalog.Products.Commands.UpdateProduct;
using MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

namespace MediSearch.Presentation.WebApi.Catalog.Products;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductRequest, CreateProductCommand>();
        config.NewConfig<UpdateProductRequest, UpdateProductCommand>().Ignore(dest => dest.Id);
    }
}
