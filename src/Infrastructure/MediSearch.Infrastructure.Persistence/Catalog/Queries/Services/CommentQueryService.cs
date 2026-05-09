using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Services;

internal sealed class CommentQueryService(IDbConnectionFactory db) : ICommentQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<CommentPostedForCompanyDto> GetCommentPostedForCompanyAsync(
        Guid productId,
        Guid authorUserId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<CommentPostedForCompanyDto>(
            CommentSql.GetCommentPostedForCompany,
            new { ProductId = productId, AuthorUserId = authorUserId }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"CommentPostedForCompany data with product '{productId}' and author '{authorUserId}'"
                    + $" was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<CommentReplyForAuthorDto> GetCommentReplyForAuthorAsync(
        Guid parentCommentId,
        Guid replyAuthorUserId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<CommentReplyForAuthorDto>(
            CommentSql.GetCommentReplyForAuthor,
            new { ParentCommentId = parentCommentId, ReplyAuthorUserId = replyAuthorUserId }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"CommentReplyForAuthor data with comment '{parentCommentId}' "
                    + $"and author '{replyAuthorUserId}' was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<IReadOnlyList<CommentDto>> GetCommentRepliesAsync(
        Guid commentId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var replies = await conn.QueryAsync<CommentDto, AuthorDto, AuthorCompanyDto, CommentDto>(
            CommentSql.GetCommentReplies,
            (reply, author, company) => reply with { Author = author with { Company = company } },
            new { CommentId = commentId },
            splitOn: $"{nameof(AuthorDto.Id)},{nameof(AuthorCompanyDto.Id)}"
        );

        return [.. replies];
    }

    public async Task<AuthorDto> GetCommentAuthorAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QueryAsync<AuthorDto, AuthorCompanyDto, AuthorDto>(
            CommentSql.GetCommentAuthor,
            (author, company) => author with { Company = company },
            new { UserId = userId },
            splitOn: nameof(AuthorCompanyDto.Id)
        );

        var author = result.SingleOrDefault();

        if (author is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Comment author for user '{userId}' was expected to exist but was not found."
            );
        }

        return author;
    }
}
