using System.Globalization;

namespace MediSearch.Infrastructure.Localization.Configuration.Culture;

internal sealed class HttpCultureProvider : ICultureProvider
{
    public CultureInfo GetCurrentCulture()
    {
        return CultureInfo.CurrentUICulture;
    }
}
