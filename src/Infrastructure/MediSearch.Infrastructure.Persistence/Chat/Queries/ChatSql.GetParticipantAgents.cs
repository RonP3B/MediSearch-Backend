using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal static partial class ChatSql
{
    public const string GetParticipantAgents =
        $@"
        -- Resolve both chat room participants as user/company agents in one query.
        SELECT
            crp.participant_id AS {nameof(AgentSummaryDto.AgentId)},
            crp.participant_type_id AS {nameof(AgentSummaryDto.AgentTypeId)},
            COALESCE(u.username, c.name) AS {nameof(AgentSummaryDto.Name)},
            COALESCE(u.profile_image_key, c.image_key) AS {nameof(AgentSummaryDto.ImageKey)}
        FROM chat_room_participants crp
        LEFT JOIN users u
            ON u.id = crp.participant_id
           AND crp.participant_type_id = @UserAgentTypeId
        LEFT JOIN companies c
            ON c.id = crp.participant_id
           AND crp.participant_type_id = @CompanyAgentTypeId
        WHERE crp.chat_room_id = @ChatRoomId
        ORDER BY crp.participant_id;
        ";
}
