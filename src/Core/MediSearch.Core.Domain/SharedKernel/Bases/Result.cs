using System.Diagnostics.CodeAnalysis;

namespace MediSearch.Core.Domain.SharedKernel.Bases;

public sealed class Result<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }
    public IReadOnlyList<DomainFailure> Failures { get; }
    public T? Value { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Failures = [];
    }

    private Result(IEnumerable<DomainFailure> failures)
    {
        IsSuccess = false;
        Failures = [.. failures];
    }

    public static Result<T> Ok(T value) => new(value);

    public static Result<T> Fail(params DomainFailure[] failures) => new(failures);

    public static Result<T> Fail(IEnumerable<DomainFailure> failures) => new(failures);
}
