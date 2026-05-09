using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.CreateChatRoom;

public sealed class CreateChatRoomCommandHandler(
    IChatRoomRepository chatRoomRepository,
    ICurrentUser currentUser,
    IAgentQueryService agentQueryService
) : ICommandHandler<CreateChatRoomCommand, ChatRoomCreatedDto>
{
    private readonly IChatRoomRepository _chatRoomRepository = chatRoomRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAgentQueryService _agentQueryService = agentQueryService;

    public async Task<ChatRoomCreatedDto> Handle(
        CreateChatRoomCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        Agent recipientAgent = Agent.From(cmd.RecipientAgentTypeId, cmd.RecipientAgentId);

        ChatRoom chatRoom = ChatRoom.Create(currentAgent, recipientAgent);

        _chatRoomRepository.Add(chatRoom);

        return chatRoom.Adapt<ChatRoomCreatedDto>() with
        {
            Recipient = await _agentQueryService.GetAgentSummaryAsync(
                recipientAgent.AgentId,
                recipientAgent.AgentTypeId,
                cancellationToken
            ),
        };
    }
}
