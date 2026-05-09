namespace MediSearch.Core.Domain.Companies;

public sealed class CompanyType : ISmartEnum<CompanyType>
{
    private CompanyType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }

    public static readonly CompanyType Pharmacy = new(1, nameof(Pharmacy));
    public static readonly CompanyType Laboratory = new(2, nameof(Laboratory));

    public static IReadOnlyCollection<CompanyType> List() => All;

    public static CompanyType GetById(int id)
    {
        return All.FirstOrDefault(companyType => companyType.Id == id)
            ?? throw new BusinessRuleException(
                nameof(CompanyType),
                new ErrorCode(
                    DomainErrorCodes.EnumerationIdNotFound,
                    new() { ["Id"] = id.ToString() }
                )
            );
    }

    public static CompanyType GetByName(string name)
    {
        return All.FirstOrDefault(companyType => companyType.Name == name)
            ?? throw new BusinessRuleException(
                nameof(CompanyType),
                new ErrorCode(DomainErrorCodes.EnumerationNameNotFound, new() { ["Name"] = name })
            );
    }

    private static readonly IReadOnlyCollection<CompanyType> All = [Pharmacy, Laboratory];
}
