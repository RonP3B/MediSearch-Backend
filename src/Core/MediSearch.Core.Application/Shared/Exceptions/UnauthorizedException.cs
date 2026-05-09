namespace MediSearch.Core.Application.Shared.Exceptions;

using MediSearch.Core.Application.Shared.Constants;
using MediSearch.Core.Domain.SharedKernel.Bases;

public sealed class UnauthorizedException : Exception
{
    public ErrorCode ErrorCode { get; }

    public UnauthorizedException()
        : base(ApplicationErrorCodes.UnauthorizedAccess)
    {
        ErrorCode = ApplicationErrorCodes.UnauthorizedAccess;
    }

    public UnauthorizedException(ErrorCode errorCode)
        : base(errorCode.Key)
    {
        ErrorCode = errorCode;
    }
}
