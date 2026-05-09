using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.Users.ValueObjects;

public sealed partial class Username : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 15;

    private Username(string value)
    {
        Value = value;
        Normalized = value.ToLowerInvariant();
    }

    public static Result<Username> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Username>.Fail([new(nameof(Username), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (value.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(Username),
                    new ErrorCode(
                        UserErrorCodes.UsernameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(Username),
                    new ErrorCode(
                        UserErrorCodes.UsernameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!UsernameRegex().IsMatch(value))
        {
            failures.Add(new(nameof(Username), UserErrorCodes.UsernameInvalidCharacters));
        }

        return failures.Count > 0
            ? Result<Username>.Fail(failures)
            : Result<Username>.Ok(new Username(value));
    }

    public static Username From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Username>();
        }

        return result.Value;
    }

    public string Value { get; private set; }
    public string Normalized { get; private set; }

    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$", RegexOptions.IgnoreCase)]
    private static partial Regex UsernameRegex();

    public static implicit operator string(Username username) => username.ToString();

    public override string ToString() => Value;

    internal static Username Empty { get; } = new Username(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Normalized;
    }
}
