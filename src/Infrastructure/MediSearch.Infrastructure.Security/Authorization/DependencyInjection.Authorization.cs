using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Security.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private static void AddAuthorizationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
    }
}
