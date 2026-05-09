using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Catalog.Repositories;

internal sealed class CommentRepository(AppDbContext dbContext) : ICommentRepository
{
    public async Task<Comment?> GetByIdOrDefaultAsync(
        EntityId<Comment> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Comments.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        EntityId<Comment> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Comments.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public void Add(Comment comment)
    {
        dbContext.Comments.Add(comment);
    }
}
