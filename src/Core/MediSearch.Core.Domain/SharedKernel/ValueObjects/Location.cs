namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed class Location : ValueObject
{
    private const int ProvinceMaxLength = 200;
    private const int MunicipalityMaxLength = 200;
    private const int AddressMaxLength = 500;

    private Location(string province, string municipality, string address)
    {
        Province = province;
        Municipality = municipality;
        Address = address;
    }

    public static Result<Location> TryFrom(string province, string municipality, string address)
    {
        var failures = new List<DomainFailure>();

        if (string.IsNullOrWhiteSpace(province))
        {
            failures.Add(new(nameof(Province), DomainErrorCodes.EmptyField));
        }

        if (string.IsNullOrWhiteSpace(municipality))
        {
            failures.Add(new(nameof(Municipality), DomainErrorCodes.EmptyField));
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            failures.Add(new(nameof(Address), DomainErrorCodes.EmptyField));
        }

        if (failures.Count > 0)
        {
            return Result<Location>.Fail(failures);
        }

        province = province.Trim();
        municipality = municipality.Trim();
        address = address.Trim();

        if (province.Length > ProvinceMaxLength)
        {
            failures.Add(
                new(
                    nameof(Province),
                    new ErrorCode(
                        DomainErrorCodes.ProvinceTooLong,
                        new() { ["MaxLength"] = $"{ProvinceMaxLength}" }
                    )
                )
            );
        }

        if (municipality.Length > MunicipalityMaxLength)
        {
            failures.Add(
                new(
                    nameof(Municipality),
                    new ErrorCode(
                        DomainErrorCodes.MunicipalityTooLong,
                        new() { ["MaxLength"] = $"{MunicipalityMaxLength}" }
                    )
                )
            );
        }

        if (address.Length > AddressMaxLength)
        {
            failures.Add(
                new(
                    nameof(Address),
                    new ErrorCode(
                        DomainErrorCodes.AddressTooLong,
                        new() { ["MaxLength"] = $"{AddressMaxLength}" }
                    )
                )
            );
        }

        return failures.Count > 0
            ? Result<Location>.Fail(failures)
            : Result<Location>.Ok(new Location(province, municipality, address));
    }

    public static Location From(string province, string municipality, string address)
    {
        var result = TryFrom(province, municipality, address);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Location>();
        }

        return result.Value;
    }

    public string Province { get; private set; }
    public string Municipality { get; private set; }
    public string Address { get; private set; }

    public override string ToString() => FullLocation;

    internal static Location Empty { get; } =
        new Location(string.Empty, string.Empty, string.Empty);

    private string FullLocation => $"{Address}, {Municipality}, {Province}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Province;
        yield return Municipality;
        yield return Address;
    }
}
