using MediSearch.Core.Application.Shared.Ports;

namespace MediSearch.Infrastructure.Base.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
