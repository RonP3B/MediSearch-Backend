using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.MarkChatRoomAsRead;

public sealed class MarkChatRoomAsReadCommandHandler(
    IChatRoomRepository chatRoomRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<MarkChatRoomAsReadCommand>
{
    private readonly IChatRoomRepository _chatRoomRepository = chatRoomRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task Handle(MarkChatRoomAsReadCommand cmd, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRoomRepository.GetByIdOrDefaultAsync(
            EntityId<ChatRoom>.From(cmd.ChatRoomId),
            cancellationToken
        );

        if (chatRoom == null)
        {
            throw NotFoundException.Entity(nameof(ChatRoom), nameof(ChatRoom.Id), cmd.ChatRoomId);
        }

        chatRoom.MarkAsRead(_currentUser.ToAgent(), _dateTimeProvider.UtcNow);
    }
}
