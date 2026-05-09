using MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentToProduct;
using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

namespace MediSearch.Presentation.WebApi.Catalog.Comments.Endpoints;

internal sealed class AddCommentToProductEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddCommentToProduct);
    }

    [EndpointSummary("Add Comment To Product")]
    [EndpointDescription("Adds a comment to a product.")]
    public static async Task<Created<CommentDto>> AddCommentToProduct(
        AddCommentToProductRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdComment = await sender.Send(
            new AddCommentToProductCommand(request.ProductId, request.Content)
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetProductDetailsByIdEndpoint.GetProductDetailsById),
                new { request.ProductId }
            ),
            createdComment
        );
    }
}

internal sealed record AddCommentToProductRequest
{
    public required Guid ProductId { get; init; }
    public required string Content { get; init; }
}
