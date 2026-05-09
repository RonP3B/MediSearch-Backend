using System.Globalization;

namespace MediSearch.Infrastructure.Localization.Configuration.Json;

internal sealed class JsonLocalizationOptions
{
    public string[] ResourcesPaths { get; set; } = [];
    public CultureInfo DefaultCulture { get; set; } = CultureInfo.InvariantCulture;
    public HashSet<CultureInfo> SupportedCultures { get; set; } = [];
}
