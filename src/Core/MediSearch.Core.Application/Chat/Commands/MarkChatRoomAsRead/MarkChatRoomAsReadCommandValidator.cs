using FluentValidation;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.MarkChatRoomAsRead;

public sealed class MarkChatRoomAsReadCommandValidator
    : AbstractValidator<MarkChatRoomAsReadCommand>
{
    public MarkChatRoomAsReadCommandValidator()
    {
        RuleFor(v => v.ChatRoomId).ValidValueObject(EntityId<ChatRoom>.TryFrom);
    }
}
