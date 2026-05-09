using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Infrastructure.Localization.Configuration.Culture;
using MediSearch.Infrastructure.Localization.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace MediSearch.Infrastructure.Localization.Configuration.Localizers.Error;

internal sealed class JsonErrorLocalizer(
    JsonLocalizationStore store,
    ICultureProvider cultureProvider,
    ILogger<JsonErrorLocalizer> logger
) : IErrorLocalizer
{
    public string this[ErrorCode errorCode]
    {
        get
        {
            var culture = cultureProvider.GetCurrentCulture();
            var resources = store.GetResources(culture);

            if (!resources.TryGetValue(errorCode.Key, out var template))
            {
                logger.LogError(
                    "Missing translation for key '{Key}' in culture '{Culture}'",
                    errorCode.Key,
                    culture.Name
                );

                return errorCode.Key;
            }

            return ApplyParameters(template, errorCode.Parameters);
        }
    }

    private static string ApplyParameters(
        string template,
        IReadOnlyDictionary<string, string> parameters
    )
    {
        if (parameters.Count == 0)
        {
            return template;
        }

        var result = template;

        foreach (var kv in parameters)
        {
            result = result.Replace($"{{{kv.Key}}}", kv.Value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
