namespace MediSearch.Core.Application.Shared.Ports;

public interface IAgentQueryService : IQueryService
{
    Task<AgentSummaryDto> GetAgentSummaryAsync(
        Guid agentId,
        int agentTypeId,
        CancellationToken cancellationToken
    );

    Task<bool> AgentExistsAsync(Guid agentId, int agentTypeId, CancellationToken cancellationToken);
}
