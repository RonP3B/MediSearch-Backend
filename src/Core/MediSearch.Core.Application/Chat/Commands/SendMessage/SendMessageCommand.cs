using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Core.Application.Chat.Commands.SendMessage;

[Authorize]
public sealed record SendMessageCommand(
    Guid ChatRoomId,
    string? TextContent = null,
    FileDto? Attachment = null
) : ICommand<MessageDto>;
