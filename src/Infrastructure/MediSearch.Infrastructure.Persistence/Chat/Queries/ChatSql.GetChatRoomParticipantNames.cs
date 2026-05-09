using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal static partial class ChatSql
{
    public const string GetChatRoomParticipantNames =
        $@"
        SELECT
            company.name AS {nameof(ChatRoomParticipantNamesDto.CompanyName)},
            COALESCE(
                -- Company users chat on behalf of their company, so when the counterparty
                -- is a normal client we expose his username.
                NULLIF(other_user.username, ''),
                other_company.name
            ) AS {nameof(ChatRoomParticipantNamesDto.CounterpartyName)}
        FROM chat_room_participants company_participant

        -- Anchor on the recipient company participant. This avoids ambiguity when both
        -- chat room participants are companies.
        INNER JOIN companies company
            ON company.id = company_participant.participant_id
           AND company_participant.participant_type_id = @CompanyAgentTypeId

        -- Get the other participant in the same room. This represents either:
        --   1. a normal client user, or
        --   2. another company (pharmacy/laboratory).
        INNER JOIN chat_room_participants other_participant
            ON other_participant.chat_room_id = company_participant.chat_room_id
           AND (
                other_participant.participant_type_id != company_participant.participant_type_id
                OR other_participant.participant_id != company_participant.participant_id
           )

        -- Resolve the polymorphic 'agent' representation into the appropriate display source.
        LEFT JOIN users other_user
            ON other_user.id = other_participant.participant_id
           AND other_participant.participant_type_id = @UserAgentTypeId
        LEFT JOIN companies other_company
            ON other_company.id = other_participant.participant_id
           AND other_participant.participant_type_id = @CompanyAgentTypeId
        WHERE company_participant.chat_room_id = @ChatRoomId
          AND company_participant.participant_id = @RecipientCompanyId
        LIMIT 1;
        ";
}
