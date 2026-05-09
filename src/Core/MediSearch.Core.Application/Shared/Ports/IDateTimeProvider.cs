namespace MediSearch.Core.Application.Shared.Ports;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
