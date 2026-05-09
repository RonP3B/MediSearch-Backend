namespace MediSearch.Core.Domain.SharedKernel.Enums;

public sealed class AgentType : ISmartEnum<AgentType>
{
    private AgentType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }

    public static readonly AgentType User = new(1, nameof(User));
    public static readonly AgentType Company = new(2, nameof(Company));

    internal static readonly AgentType Empty = new(0, string.Empty);

    public static IReadOnlyCollection<AgentType> List() => All;

    public static AgentType GetById(int id)
    {
        return All.FirstOrDefault(at => at.Id == id)
            ?? throw new BusinessRuleException(
                nameof(AgentType),
                new ErrorCode(
                    DomainErrorCodes.EnumerationIdNotFound,
                    new() { ["Id"] = id.ToString() }
                )
            );
    }

    public static AgentType GetByName(string name)
    {
        return All.FirstOrDefault(at => at.Name == name)
            ?? throw new BusinessRuleException(
                nameof(AgentType),
                new ErrorCode(DomainErrorCodes.EnumerationNameNotFound, new() { ["Name"] = name })
            );
    }

    private static readonly IReadOnlyCollection<AgentType> All = [User, Company];
}
