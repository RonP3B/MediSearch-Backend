using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal static partial class ChatSql
{
    public const string GetChatRoomList =
        $@"
        SELECT
            cr.id                                                   AS {nameof(ChatListItemDto.Id)},
            m.text_content                                          AS {nameof(ChatListItemDto.LastMessageText)},                            
            m.media_content_asset_key                               AS {nameof(ChatListItemDto.LastMessageMediaAssetKey)},   
            m.media_content_type                                    AS {nameof(ChatListItemDto.LastMessageMediaType)},           
            m.message_sent_date                                     AS {nameof(ChatListItemDto.LastMessageSentDate)},
            
            -- Count messages in this chat room that:
            --   1. Were not sent by the current agent (own messages are never unread,
            --      regardless of last_checked_at -- defensive guard in case the checkpoint
            --      is not updated on every send)
            --   2. Were sent after the current agent last checked the chat,
            --      or count all if the agent has never checked (last_checked_at IS NULL)
            (
                SELECT COUNT(*)
                FROM messages msg
                WHERE msg.chat_room_id = cr.id
                  AND NOT (
                        msg.sender_id = @CurrentAgentId
                        AND msg.sender_type_id = @CurrentAgentTypeId
                  )
                  AND (
                     crp_current.last_checked_at IS NULL
                     OR msg.message_sent_date > crp_current.last_checked_at
                  )
            )                                                       AS {nameof(ChatListItemDto.UnreadCount)},

            -- Resolve recipient display info from either users or companies
            -- depending on the other participant's agent type
            COALESCE(u.id, c.id)                                    AS {nameof(AgentSummaryDto.AgentId)},
            crp_other.participant_type_id                           AS {nameof(AgentSummaryDto.AgentTypeId)},
            COALESCE(u.username, c.name)                            AS {nameof(AgentSummaryDto.Name)},
            COALESCE(u.profile_image_key, c.image_key)              AS {nameof(AgentSummaryDto.ImageKey)}

        FROM chat_rooms cr

        -- Fetch the last message preview (nullable, chat may have no messages yet)
        LEFT JOIN messages m ON m.id = cr.last_message_id

        -- Anchor to chat rooms where the current agent is a participant
        -- and pull last_checked_at for the unread count subquery above
        INNER JOIN chat_room_participants crp_current ON crp_current.chat_room_id = cr.id
           AND crp_current.participant_id = @CurrentAgentId
           AND crp_current.participant_type_id = @CurrentAgentTypeId

        -- Find the other participant (recipient) in the same chat room
        INNER JOIN chat_room_participants crp_other ON crp_other.chat_room_id = cr.id
           AND NOT (
               crp_other.participant_id = @CurrentAgentId
               AND crp_other.participant_type_id = @CurrentAgentTypeId
           )

        -- Resolve recipient as user or company based on agent type (mutually exclusive)
        LEFT JOIN users u ON u.id = crp_other.participant_id
           AND crp_other.participant_type_id = @UserAgentTypeId
        LEFT JOIN companies c ON c.id = crp_other.participant_id
           AND crp_other.participant_type_id = @CompanyAgentTypeId

        ORDER BY m.message_sent_date DESC NULLS LAST;
        ";
}
