using MediSearch.Presentation.WebApi.Shared.OpenApi;

namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    public static void AddOpenApiServices(this IHostApplicationBuilder builder)
    {
        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true
        );

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<ApiExceptionOperationTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }
}
