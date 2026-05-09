namespace MediSearch.Core.Domain.Chat.ChatRooms;

public sealed class ChatRoomParticipant
{
    private ChatRoomParticipant() { }

    public ChatRoomParticipant(Agent agent)
    {
        Agent = agent;
    }

    public Agent Agent { get; private set; } = Agent.Empty;
    public DateTimeOffset? LastCheckedAt { get; private set; }

    public void UpdateLastChecked(DateTimeOffset now)
    {
        LastCheckedAt = now;
    }
}
