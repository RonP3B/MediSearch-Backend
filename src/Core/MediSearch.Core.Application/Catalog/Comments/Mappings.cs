using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Domain.Catalog.Comments;

namespace MediSearch.Core.Application.Catalog.Comments;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Comment, CommentDto>().Ignore(dest => dest.Author);
    }
}
