using System.Reflection;

namespace MediSearch.Presentation.WebApi.Shared.Endpoints;

internal static class EndpointMappingExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var routeGroups = new Dictionary<(string RoutePrefix, string Tag), RouteGroupBuilder>();

        var endpointTypes = assembly
            .GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsInterface: false }
                && t.IsAssignableTo(typeof(IEndpoint))
            )
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name);

        foreach (var type in endpointTypes)
        {
            if (Activator.CreateInstance(type, nonPublic: true) is not IEndpoint endpoint)
            {
                continue;
            }

            var featureName = ResolveFeatureName(type);
            var routePrefix =
                endpoint.RoutePrefix ?? $"/api/v1/{ConvertFeaturePathToRoutePath(featureName)}";
            var tag = endpoint.Tag ?? featureName.Split('.').Last();
            var key = (routePrefix, tag);

            if (!routeGroups.TryGetValue(key, out var group))
            {
                group = app.MapGroup(routePrefix).WithTags(tag);
                routeGroups[key] = group;
            }

            endpoint.Map(group);
        }

        return app;
    }

    private static string ResolveFeatureName(Type type)
    {
        const string rootNamespace = "MediSearch.Presentation.WebApi.";

        if (type.Namespace is not { Length: > 0 } endpointNamespace)
        {
            return type.Name;
        }

        if (endpointNamespace.StartsWith(rootNamespace, StringComparison.Ordinal))
        {
            endpointNamespace = endpointNamespace[rootNamespace.Length..];
        }

        const string endpointsSuffix = ".Endpoints";
        int endpointsIndex = endpointNamespace.IndexOf(endpointsSuffix, StringComparison.Ordinal);

        if (endpointsIndex >= 0)
        {
            return endpointNamespace[..endpointsIndex];
        }

        return endpointNamespace.Split('.').First();
    }

    private static string ConvertFeaturePathToRoutePath(string featurePath) =>
        string.Join("/", featurePath.Split('.').Select(ConvertToKebabCase));

    private static string ConvertToKebabCase(string input)
    {
        return string.Concat(
            input.Select(
                (x, i) =>
                    i > 0 && char.IsUpper(x) ? "-" + char.ToLower(x) : char.ToLower(x).ToString()
            )
        );
    }
}
