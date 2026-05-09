namespace MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;

public interface ITextLocalizer
{
    string this[string key] { get; }
    string this[string key, params object[] args] { get; }
}
