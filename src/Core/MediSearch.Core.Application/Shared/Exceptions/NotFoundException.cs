using MediSearch.Core.Domain.SharedKernel.Bases;

namespace MediSearch.Core.Application.Shared.Exceptions;

public sealed class NotFoundException : Exception
{
    public ErrorCode ErrorCode { get; }

    public NotFoundException()
        : base(ApplicationErrorCodes.ResourceNotFound)
    {
        ErrorCode = ApplicationErrorCodes.ResourceNotFound;
    }

    public NotFoundException(ErrorCode errorCode)
        : base(errorCode.Key)
    {
        ErrorCode = errorCode;
    }

    public static NotFoundException Entity<T>(string entity, string field, T value)
    {
        return new(
            new ErrorCode(
                ApplicationErrorCodes.EntityNotFound,
                new Dictionary<string, string>
                {
                    ["Entity"] = entity.ToLowerInvariant(),
                    ["Field"] = field.ToLowerInvariant(),
                    ["Value"] = value?.ToString() ?? "null",
                }
            )
        );
    }
}
