namespace MediSearch.Core.Domain.Catalog.ProductClassifications;

public sealed class Name : ValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 150;

    private Name(string value)
    {
        Value = value;
        Normalized = value.ToLowerInvariant();
    }

    public static Result<Name> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Name>.Fail([new(nameof(Name), DomainErrorCodes.EmptyField)]);
        }

        value = value.Trim();

        var failures = new List<DomainFailure>();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(Name),
                    new ErrorCode(
                        ProductClassificationErrorCodes.NameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (value.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(Name),
                    new ErrorCode(
                        ProductClassificationErrorCodes.NameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        return failures.Count > 0 ? Result<Name>.Fail(failures) : Result<Name>.Ok(new Name(value));
    }

    public static Name From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Name>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    /// <summary>
    /// Lower-cased form of the value. It is what uniqueness checks, lookups and equality
    /// use, so two values that differ only in casing are the same value, while
    /// <see cref="Value"/> still holds exactly what the user typed.
    /// </summary>
    public string Normalized { get; private set; }

    public static implicit operator string(Name name) => name.Value;

    public override string ToString() => Value;

    internal static Name Empty { get; } = new(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Normalized;
    }
}
