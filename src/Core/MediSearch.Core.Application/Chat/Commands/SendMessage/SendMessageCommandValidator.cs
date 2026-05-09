using FluentValidation;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(v => v.ChatRoomId).ValidValueObject(EntityId<ChatRoom>.TryFrom);

        RuleFor(v => v.TextContent)
            .ValidValueObject(CleanText.TryFrom!)
            .When(v => !string.IsNullOrEmpty(v.TextContent) || v.Attachment is null);

        RuleFor(v => v.Attachment)
            .MediaFile()
            .WithMaxSizeMB(10)
            .When(v => v.Attachment is not null || string.IsNullOrEmpty(v.TextContent));
    }
}
