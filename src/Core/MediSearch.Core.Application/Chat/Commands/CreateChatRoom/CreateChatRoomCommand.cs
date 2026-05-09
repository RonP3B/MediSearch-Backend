using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Core.Application.Chat.Commands.CreateChatRoom;

[Authorize]
public sealed record CreateChatRoomCommand(Guid RecipientAgentId, int RecipientAgentTypeId)
    : ICommand<ChatRoomCreatedDto>;
