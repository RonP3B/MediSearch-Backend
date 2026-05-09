using MediSearch.Core.Domain.SharedKernel.Bases;

namespace MediSearch.Infrastructure.Localization.Configuration.Localizers.Error;

public interface IErrorLocalizer
{
    string this[ErrorCode errorCode] { get; }
}
