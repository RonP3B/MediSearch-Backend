namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal static partial class ChatSql
{
    public const string GetParticipantChatRoomIds =
        @"
        SELECT crp.chat_room_id
        FROM chat_room_participants crp
        WHERE crp.participant_id = @ParticipantId
          AND crp.participant_type_id = @ParticipantTypeId;
        ";
}
