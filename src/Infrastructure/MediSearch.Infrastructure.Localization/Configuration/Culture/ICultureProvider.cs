using System.Globalization;

namespace MediSearch.Infrastructure.Localization.Configuration.Culture;

public interface ICultureProvider
{
    CultureInfo GetCurrentCulture();
}
