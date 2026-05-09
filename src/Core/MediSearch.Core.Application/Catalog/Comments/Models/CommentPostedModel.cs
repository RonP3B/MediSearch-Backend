namespace MediSearch.Core.Application.Catalog.Comments.Models;

public sealed record CommentPostedModel(
    string CompanyName,
    string ProductName,
    string CommentAuthorName,
    string CommentContent
);
