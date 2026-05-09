using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal static partial class ChatSql
{
    public const string GetChatRoomMessages =
        $@"
        -- 1) Recipient info
        SELECT
            COALESCE(u.id, c.id)                       AS {nameof(AgentSummaryDto.AgentId)},
            crp.participant_type_id                    AS {nameof(AgentSummaryDto.AgentTypeId)},
            COALESCE(u.username, c.name)               AS {nameof(AgentSummaryDto.Name)},
            COALESCE(u.profile_image_key, c.image_key) AS {nameof(AgentSummaryDto.ImageKey)}
        FROM chat_room_participants crp
        LEFT JOIN users u ON u.id = crp.participant_id
           AND crp.participant_type_id = @UserAgentTypeId
        LEFT JOIN companies c ON c.id = crp.participant_id
           AND crp.participant_type_id = @CompanyAgentTypeId
        WHERE crp.chat_room_id = @ChatRoomId
          AND NOT (
              crp.participant_id = @CurrentAgentId
              AND crp.participant_type_id = @CurrentAgentTypeId
          );
 
        -- 2) Messages with sender info
        SELECT
            m.id                                               AS {nameof(MessageDto.Id)},
            m.chat_room_id                                     AS {nameof(MessageDto.ChatRoomId)},
            m.text_content                                     AS {nameof(MessageDto.TextContent)},
            m.media_content_asset_key                          AS {nameof(MessageDto.MediaContentAssetKey)},
            m.media_content_type                               AS {nameof(MessageDto.MediaContentType)},
            m.message_sent_date                                AS {nameof(MessageDto.MessageSentDate)},
            COALESCE(u.id, c.id)                               AS {nameof(AgentSummaryDto.AgentId)},
            m.sender_type_id                                   AS {nameof(AgentSummaryDto.AgentTypeId)},
            COALESCE(u.username, c.name)                       AS {nameof(AgentSummaryDto.Name)},
            COALESCE(u.profile_image_key, c.image_key)         AS {nameof(AgentSummaryDto.ImageKey)}
        FROM messages m
        LEFT JOIN users u ON u.id = m.sender_id
           AND m.sender_type_id = @UserAgentTypeId
        LEFT JOIN companies c ON c.id = m.sender_id
           AND m.sender_type_id = @CompanyAgentTypeId
        WHERE m.chat_room_id = @ChatRoomId
        ORDER BY m.message_sent_date ASC;
        ";
}
