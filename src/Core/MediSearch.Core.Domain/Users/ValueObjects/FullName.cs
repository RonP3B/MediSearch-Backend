using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.Users.ValueObjects;

public sealed partial class FullName : ValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 100;

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<FullName> TryFrom(string firstName, string lastName)
    {
        var failures = new List<DomainFailure>();

        if (string.IsNullOrWhiteSpace(firstName))
        {
            failures.Add(new(nameof(FirstName), DomainErrorCodes.EmptyField));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            failures.Add(new(nameof(LastName), DomainErrorCodes.EmptyField));
        }

        if (failures.Count > 0)
        {
            return Result<FullName>.Fail(failures);
        }

        firstName = firstName.Trim().ToLowerInvariant();
        lastName = lastName.Trim().ToLowerInvariant();

        if (firstName.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(FirstName),
                    new ErrorCode(
                        UserErrorCodes.FirstNameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (lastName.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(LastName),
                    new ErrorCode(
                        UserErrorCodes.LastNameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (firstName.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(FirstName),
                    new ErrorCode(
                        UserErrorCodes.FirstNameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (lastName.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(LastName),
                    new ErrorCode(
                        UserErrorCodes.LastNameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!ValidNameRegex().IsMatch(firstName))
        {
            failures.Add(new(nameof(FirstName), UserErrorCodes.InvalidFirstName));
        }

        if (!ValidNameRegex().IsMatch(lastName))
        {
            failures.Add(new(nameof(LastName), UserErrorCodes.InvalidLastName));
        }

        return failures.Count > 0
            ? Result<FullName>.Fail(failures)
            : Result<FullName>.Ok(new FullName(firstName, lastName));
    }

    public static FullName From(string firstName, string lastName)
    {
        var result = TryFrom(firstName, lastName);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<FullName>();
        }

        return result.Value;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public override string ToString() => $"{FirstName} {LastName}";

    internal static FullName Empty { get; } = new FullName(string.Empty, string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚÑáéíóúñ\s]+$", RegexOptions.Compiled)]
    private static partial Regex ValidNameRegex();
}
