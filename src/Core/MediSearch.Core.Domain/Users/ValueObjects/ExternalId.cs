namespace MediSearch.Core.Domain.Users.ValueObjects;

public sealed class ExternalId : ValueObject
{
    private const int MaxLength = 200;

    private ExternalId(string value)
    {
        Value = value;
    }

    public static Result<ExternalId> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ExternalId>.Fail([new(nameof(ExternalId), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(ExternalId),
                    new ErrorCode(
                        UserErrorCodes.ExternalIdTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        return failures.Count > 0
            ? Result<ExternalId>.Fail(failures)
            : Result<ExternalId>.Ok(new ExternalId(value));
    }

    public static ExternalId From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<ExternalId>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    public static implicit operator string(ExternalId id) => id.Value;

    public override string ToString() => Value;

    internal static ExternalId Empty { get; } = new ExternalId(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
