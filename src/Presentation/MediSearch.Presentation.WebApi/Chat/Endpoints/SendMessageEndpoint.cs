using MediSearch.Core.Application.Chat.Commands.SendMessage;
using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Presentation.WebApi.Chat.Endpoints;

internal sealed class SendMessageEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(SendMessage, "{chatRoomId:guid}/messages").DisableAntiforgery();
    }

    [EndpointSummary("Send Message")]
    [EndpointDescription("Sends a message to a chat room.")]
    public static async Task<Created<MessageDto>> SendMessage(
        Guid chatRoomId,
        [FromForm] SendMessageRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdMessage = await sender.Send(
            new SendMessageCommand(
                chatRoomId,
                request.TextContent,
                request.Attachment?.Adapt<FileDto>()
            )
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetChatRoomMessagesEndpoint.GetChatRoomMessages),
                new { chatRoomId }
            ),
            createdMessage
        );
    }
}

internal sealed record SendMessageRequest
{
    public string? TextContent { get; init; } = null;
    public IFormFile? Attachment { get; init; } = null;
}
