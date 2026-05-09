namespace MediSearch.Core.Application.Shared.Exceptions;

using MediSearch.Core.Application.Shared.Constants;
using MediSearch.Core.Domain.SharedKernel.Bases;

public sealed class ForbiddenAccessException : Exception
{
    public ErrorCode ErrorCode { get; }

    public ForbiddenAccessException()
        : base(ApplicationErrorCodes.ForbiddenAccess)
    {
        ErrorCode = ApplicationErrorCodes.ForbiddenAccess;
    }
}
