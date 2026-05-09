using MediSearch.Infrastructure.Localization.Configuration.Culture;
using MediSearch.Infrastructure.Localization.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;

internal sealed class JsonTextLocalizer(
    JsonLocalizationStore store,
    ICultureProvider cultureProvider,
    ILogger<JsonTextLocalizer> logger
) : ITextLocalizer
{
    public string this[string key]
    {
        get
        {
            var culture = cultureProvider.GetCurrentCulture();
            var resources = store.GetResources(culture);

            if (!resources.TryGetValue(key, out var value))
            {
                logger.LogWarning(
                    "Missing template localization key '{Key}' for culture '{Culture}'",
                    key,
                    culture.Name
                );

                return key;
            }

            return value;
        }
    }

    public string this[string key, params object[] args]
    {
        get
        {
            var msg = this[key];
            return string.Format(msg, args);
        }
    }
}
