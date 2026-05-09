using System.Diagnostics.CodeAnalysis;
using MediSearch.Core.Domain.SharedKernel.Bases;

namespace MediSearch.Core.Application.Shared.Results;

public class ServiceResult
{
    internal ServiceResult(
        bool succeeded,
        IEnumerable<(string ErrorKey, ErrorCode ErrorCode)> errors
    )
    {
        Succeeded = succeeded;
        Errors = errors
            .GroupBy(e => e.ErrorKey, e => e.ErrorCode)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public bool Succeeded { get; init; }
    public IDictionary<string, ErrorCode[]> Errors { get; init; }

    public static ServiceResult Success()
    {
        return new ServiceResult(true, []);
    }

    public static ServiceResult Failure(IEnumerable<(string ErrorKey, ErrorCode ErrorCode)> errors)
    {
        return new ServiceResult(false, errors);
    }

    public static ServiceResult Failure(string errorKey, ErrorCode errorCode)
    {
        return new ServiceResult(false, [(errorKey, errorCode)]);
    }

    public bool HasError(ErrorCode errorCode)
    {
        return Errors.Values.Any(codes => codes.Any(c => c.Key == errorCode.Key));
    }
}

public class ServiceResult<T> : ServiceResult
{
    internal ServiceResult(
        bool succeeded,
        IEnumerable<(string ErrorKey, ErrorCode ErrorCode)> errors,
        T? value = default
    )
        : base(succeeded, errors)
    {
        Value = value;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public new bool Succeeded => base.Succeeded;

    public T? Value { get; }

    public static ServiceResult<T> Success(T value)
    {
        return new ServiceResult<T>(true, [], value);
    }

    public static ServiceResult<T> Failure(
        IEnumerable<(string ErrorKey, ErrorCode ErrorCode)> errors,
        T? value = default
    )
    {
        return new ServiceResult<T>(false, errors, value);
    }

    public static ServiceResult<T> Failure(string errorKey, ErrorCode errorCode, T? value)
    {
        return new ServiceResult<T>(false, [(errorKey, errorCode)], value);
    }
}
