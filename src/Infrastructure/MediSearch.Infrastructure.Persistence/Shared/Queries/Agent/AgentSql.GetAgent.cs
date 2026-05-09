using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Infrastructure.Persistence.Shared.Queries.Agent;

internal static partial class AgentSql
{
    public const string GetAgent =
        @$"
        SELECT
            COALESCE(u.id, c.id)                       AS {nameof(AgentSummaryDto.AgentId)},
            @AgentTypeId                               AS {nameof(AgentSummaryDto.AgentTypeId)},
            COALESCE(u.username, c.name)               AS {nameof(AgentSummaryDto.Name)},
            COALESCE(u.profile_image_key, c.image_key) AS {nameof(AgentSummaryDto.ImageKey)}
        FROM (SELECT @AgentId AS id, @AgentTypeId AS type_id) a
        LEFT JOIN users u ON u.id = a.id AND a.type_id = @UserAgentTypeId
        LEFT JOIN companies c ON c.id = a.id AND a.type_id = @CompanyAgentTypeId;
        ";
}
