using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Chat.Messages;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.SendMessage;

public sealed class SendMessageCommandHandler(
    IMessageRepository messageRepository,
    IChatRoomRepository chatRoomRepository,
    ICurrentUser currentUser,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager,
    IDateTimeProvider dateTimeProvider,
    IAgentQueryService agentQueryService
) : ICommandHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IChatRoomRepository _chatRoomRepository = chatRoomRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAgentQueryService _agentQueryService = agentQueryService;

    public async Task<MessageDto> Handle(
        SendMessageCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var chatRoom = await _chatRoomRepository.GetByIdOrDefaultAsync(
            EntityId<ChatRoom>.From(cmd.ChatRoomId),
            cancellationToken
        );

        if (chatRoom == null)
        {
            throw NotFoundException.Entity(nameof(ChatRoom), nameof(ChatRoom.Id), cmd.ChatRoomId);
        }

        Agent senderAgent = _currentUser.ToAgent();

        if (!chatRoom.HasParticipant(senderAgent))
        {
            throw new ForbiddenAccessException();
        }

        MessageMediaContent? mediaContent = null;

        if (cmd.Attachment is not null)
        {
            string assetKey = await _compensationManager.ExecuteAsync(
                new SaveAssetCompensableOperation(
                    fileDto: cmd.Attachment,
                    fileStorageService: _fileStorageService,
                    eventBus: _eventBus
                ),
                cancellationToken
            );

            mediaContent = MessageMediaContent.From(AssetKey.From(assetKey));
        }

        var message = Message.Create(
            chatRoomId: chatRoom.Id,
            mediaContent: mediaContent,
            senderAgent: senderAgent,
            sentAt: _dateTimeProvider.UtcNow,
            textContent: !string.IsNullOrEmpty(cmd.TextContent)
                ? CleanText.From(cmd.TextContent)
                : null
        );

        chatRoom.UpdateLastMessage(message.Id);

        _messageRepository.Add(message);

        return message.Adapt<MessageDto>() with
        {
            Sender = await _agentQueryService.GetAgentSummaryAsync(
                message.SenderAgent.AgentId,
                message.SenderAgent.AgentTypeId,
                cancellationToken
            ),
        };
    }
}
