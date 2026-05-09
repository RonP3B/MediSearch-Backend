using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Chat.Messages;

namespace MediSearch.Core.Application.Chat;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ChatRoom, ChatRoomCreatedDto>().Ignore(dest => dest.Recipient);

        config
            .NewConfig<Message, MessageDto>()
            .Ignore(dest => dest.Sender)
            .Map(
                dest => dest.MediaContentAssetKey,
                src => src.MediaContent != null ? src.MediaContent.AssetKey : null
            )
            .Map(
                dest => dest.MediaContentType,
                src => src.MediaContent != null ? src.MediaContent.MediaType.ToString() : null
            );
    }
}
