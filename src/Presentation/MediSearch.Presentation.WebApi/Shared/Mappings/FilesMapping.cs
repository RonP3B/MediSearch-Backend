using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Presentation.WebApi.Shared.Mappings;

internal sealed class FilesMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<IFormFile, FileDto>()
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.ContentType, src => src.ContentType)
            .Map(dest => dest.Content, src => src.OpenReadStream());
    }
}
