namespace MediSearch.Core.Domain.Catalog.Products.ValueObjects;

public sealed class Price : ValueObject
{
    private const double MinValue = 0.01;
    private static readonly HashSet<string> AllowedCurrencies = ["DOP", "USD", "EUR"];

    private Price(double amount, string currency)
    {
        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public static Result<Price> TryFrom(double amount, string currency = "DOP")
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return Result<Price>.Fail([new(nameof(Currency), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        currency = currency.Trim().ToUpperInvariant();

        if (amount < MinValue)
        {
            failures.Add(new(nameof(Amount), ProductErrorCodes.PriceBelowMinimum));
        }

        if (!AllowedCurrencies.Contains(currency))
        {
            failures.Add(
                new(
                    nameof(Currency),
                    new ErrorCode(
                        ProductErrorCodes.UnsupportedCurrency,
                        new() { [nameof(AllowedCurrencies)] = string.Join(", ", AllowedCurrencies) }
                    )
                )
            );
        }

        return failures.Count > 0
            ? Result<Price>.Fail(failures)
            : Result<Price>.Ok(new Price(amount, currency));
    }

    public static Price From(double amount, string currency = "DOP")
    {
        var result = TryFrom(amount, currency);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Price>();
        }

        return result.Value;
    }

    public double Amount { get; private set; }
    public string Currency { get; private set; }

    public static implicit operator double(Price price) => price.Amount;

    public override string ToString() => $"{Currency}${Amount:0.00}";

    internal static Price Empty { get; } = new Price(0.00, string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
