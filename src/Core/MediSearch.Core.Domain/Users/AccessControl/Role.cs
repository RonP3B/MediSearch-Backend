namespace MediSearch.Core.Domain.Users.AccessControl;

public sealed class Role : ISmartEnum<Role>
{
    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }

    public static readonly Role SystemAdmin = new(1, nameof(SystemAdmin));
    public static readonly Role CompanyOwner = new(2, nameof(CompanyOwner));
    public static readonly Role CompanyManager = new(3, nameof(CompanyManager));
    public static readonly Role CompanyMember = new(4, nameof(CompanyMember));
    public static readonly Role Client = new(5, nameof(Client));

    public static IReadOnlyCollection<Role> List() => All;

    public static Role GetById(int id)
    {
        return All.FirstOrDefault(r => r.Id == id)
            ?? throw new BusinessRuleException(
                nameof(Role),
                new ErrorCode(
                    DomainErrorCodes.EnumerationIdNotFound,
                    new() { ["Id"] = id.ToString() }
                )
            );
    }

    public static Role GetByName(string name)
    {
        return All.FirstOrDefault(r => r.Name == name)
            ?? throw new BusinessRuleException(
                nameof(Role),
                new ErrorCode(DomainErrorCodes.EnumerationNameNotFound, new() { ["Name"] = name })
            );
    }

    private static readonly IReadOnlyCollection<Role> All =
    [
        SystemAdmin,
        CompanyOwner,
        CompanyManager,
        CompanyMember,
        Client,
    ];
}
