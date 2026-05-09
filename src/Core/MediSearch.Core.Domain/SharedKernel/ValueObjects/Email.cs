using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed partial class Email : ValueObject
{
    private const int MaxLength = 320;

    private Email(string address)
    {
        Address = address;
    }

    public static Result<Email> TryFrom(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Result<Email>.Fail([new(nameof(Email), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        address = address.Trim().ToLowerInvariant();

        if (address.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(Email),
                    new ErrorCode(
                        DomainErrorCodes.EmailTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!EmailRegex().IsMatch(address))
        {
            failures.Add(new(nameof(Email), DomainErrorCodes.EmailInvalidFormat));
        }

        return failures.Count > 0
            ? Result<Email>.Fail(failures)
            : Result<Email>.Ok(new Email(address));
    }

    public static Email From(string address)
    {
        var result = TryFrom(address);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Email>();
        }

        return result.Value;
    }

    public string Address { get; private set; }

    public static implicit operator string(Email email) => email.ToString();

    public override string ToString() => Address;

    internal static Email Empty { get; } = new Email(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }

    [GeneratedRegex("^\\S+@\\S+\\.\\S+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
}
