using System.Text.RegularExpressions;

namespace MediSearch.Infrastructure.Base.FileStorage;

internal static partial class Extensions
{
    public static string ToFileKey(this string fileName)
    {
        return $"{Guid.NewGuid()}_{SanitizeFileName(fileName)}";
    }

    private static string SanitizeFileName(string fileName)
    {
        return SanitizeFileNameRegex().Replace(fileName, "_");
    }

    [GeneratedRegex(@"[^\w.\-]")]
    private static partial Regex SanitizeFileNameRegex();
}
