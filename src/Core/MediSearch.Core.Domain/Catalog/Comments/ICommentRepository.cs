namespace MediSearch.Core.Domain.Catalog.Comments;

public interface ICommentRepository : IRepository
{
    Task<Comment?> GetByIdOrDefaultAsync(
        EntityId<Comment> id,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(EntityId<Comment> id, CancellationToken cancellationToken = default);
    void Add(Comment comment);
}
