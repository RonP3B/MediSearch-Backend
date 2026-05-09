using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Localization.Configuration.Json;

internal sealed class JsonLocalizationStore(IOptions<JsonLocalizationOptions> options)
{
    private readonly JsonLocalizationOptions _options = options.Value;
    private readonly IFileProvider _fileProvider = new PhysicalFileProvider(
        AppContext.BaseDirectory
    );
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _cache = new(
        StringComparer.OrdinalIgnoreCase
    );

    public IReadOnlyDictionary<string, string> GetResources(CultureInfo culture)
    {
        return _cache.GetOrAdd(culture.Name, _ => LoadCulture(culture));
    }

    private Dictionary<string, string> LoadCulture(CultureInfo culture)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in _options.ResourcesPaths)
        {
            LoadCultureFiles(path, culture, result);

            if (!culture.Equals(culture.Parent) && culture.Parent != CultureInfo.InvariantCulture)
            {
                LoadCultureFiles(path, culture.Parent, result);
            }
        }

        return result;
    }

    private void LoadCultureFiles(
        string rootPath,
        CultureInfo culture,
        Dictionary<string, string> target
    )
    {
        var files = _fileProvider.GetDirectoryContents(rootPath);

        foreach (var entry in files)
        {
            if (entry.PhysicalPath is null)
            {
                continue;
            }

            if (entry.IsDirectory)
            {
                LoadCultureFiles(entry.PhysicalPath, culture, target);
                continue;
            }

            if (!entry.Name.Equals($"{culture.Name}.json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            using var stream = entry.CreateReadStream();

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);

            if (data is null)
            {
                continue;
            }

            foreach (var kv in data)
            {
                target[kv.Key] = kv.Value;
            }
        }
    }
}
