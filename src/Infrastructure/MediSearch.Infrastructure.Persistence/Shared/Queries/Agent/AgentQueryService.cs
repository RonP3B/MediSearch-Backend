using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Shared.Queries.Agent;

internal sealed class AgentQueryService(IDbConnectionFactory db) : IAgentQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<AgentSummaryDto> GetAgentSummaryAsync(
        Guid agentId,
        int agentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<AgentSummaryDto>(
            AgentSql.GetAgent,
            new
            {
                AgentId = agentId,
                AgentTypeId = agentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Agent '{agentId}' of type '{agentTypeId}' was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<bool> AgentExistsAsync(
        Guid agentId,
        int agentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        return await conn.ExecuteScalarAsync<bool>(
            AgentSql.AgentExists,
            new
            {
                AgentId = agentId,
                AgentTypeId = agentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );
    }
}
