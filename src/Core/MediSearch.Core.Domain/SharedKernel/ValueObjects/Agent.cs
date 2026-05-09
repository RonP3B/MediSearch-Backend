using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed class Agent : ValueObject
{
    private Agent(int agentTypeId, Guid agentId)
    {
        AgentTypeId = agentTypeId;
        AgentId = agentId;
    }

    public static Result<Agent> TryFrom(int agentTypeId, Guid agentId)
    {
        var failures = new List<DomainFailure>();

        if (agentId == Guid.Empty)
        {
            failures.Add(new(nameof(AgentId), DomainErrorCodes.EmptyField));
        }

        HashSet<int> agentTypeIds = [.. AgentType.List().Select(x => x.Id)];

        if (!agentTypeIds.Contains(agentTypeId))
        {
            failures.Add(new(nameof(AgentTypeId), DomainErrorCodes.InvalidAgentType));
        }

        return failures.Count > 0
            ? Result<Agent>.Fail(failures)
            : Result<Agent>.Ok(new Agent(agentTypeId, agentId));
    }

    public static Agent From(int agentTypeId, Guid agentId)
    {
        var result = TryFrom(agentTypeId, agentId);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Agent>();
        }

        return result.Value;
    }

    public int AgentTypeId { get; private set; }
    public Guid AgentId { get; private set; }

    public override string ToString() => $"{AgentTypeId}:{AgentId}";

    internal static Agent Empty { get; } = new Agent(AgentType.Empty.Id, Guid.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AgentTypeId;
        yield return AgentId;
    }
}
