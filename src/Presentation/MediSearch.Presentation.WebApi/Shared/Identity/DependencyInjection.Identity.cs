using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Presentation.WebApi.Shared.Identity;

namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    public static void AddIdentityServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
    }
}
