using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed partial class PhoneNumber : ValueObject
{
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PhoneNumber>.Fail(
                [new(nameof(PhoneNumber), DomainErrorCodes.EmptyField)]
            );
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (!ValidPhoneNumberRegex().IsMatch(value))
        {
            failures.Add(
                new(
                    nameof(PhoneNumber),
                    new ErrorCode(
                        DomainErrorCodes.PhoneNumberInvalidFormat,
                        new() { ["PhoneNumberFormat"] = "(###) ###-####" }
                    )
                )
            );
        }

        return failures.Count > 0
            ? Result<PhoneNumber>.Fail(failures)
            : Result<PhoneNumber>.Ok(new PhoneNumber(value));
    }

    public static PhoneNumber From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<PhoneNumber>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    [GeneratedRegex(@"^\([1-9][0-9]{2}\)\s\d{3}-\d{4}$")]
    private static partial Regex ValidPhoneNumberRegex();

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.ToString();

    public override string ToString() => Value;

    internal static PhoneNumber Empty { get; } = new PhoneNumber(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
