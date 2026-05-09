using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Base.Clock;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private static void AddClockServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}
