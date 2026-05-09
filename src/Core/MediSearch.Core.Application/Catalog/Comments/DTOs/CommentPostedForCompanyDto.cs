namespace MediSearch.Core.Application.Catalog.Comments.DTOs;

public sealed record CommentPostedForCompanyDto
{
    public required Guid SellerCompanyId { get; init; }
    public required string SellerCompanyName { get; init; }
    public required string ProductName { get; init; }
    public required string CommentAuthorName { get; init; }
    public Guid? AuthorCompanyId { get; init; }
}
