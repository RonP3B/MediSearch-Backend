using System.Reflection;

namespace MediSearch.Infrastructure.MessageBus.Configurations;

internal static class InstanceIdBuilder
{
    public static string Build(IHostApplicationBuilder builder)
    {
        var serviceName =
            Assembly.GetEntryAssembly()?.GetName().Name ?? builder.Environment.ApplicationName;

        return $"{serviceName}-{builder.Environment.EnvironmentName}"
            .ToLowerInvariant()
            .Replace('.', '-');
    }
}
