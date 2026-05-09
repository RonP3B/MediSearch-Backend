using MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;
using MediSearch.Core.Application.Users.Commands.RegisterUser;
using MediSearch.Core.Application.Users.Commands.UpdateUserProfile;
using MediSearch.Presentation.WebApi.Users.Endpoints;

namespace MediSearch.Presentation.WebApi.Users;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateUserProfileRequest, UpdateUserProfileCommand>();
        config.NewConfig<RegisterUserRequest, RegisterUserCommand>();
        config.NewConfig<RegisterCompanyUserRequest, RegisterCompanyUserCommand>();
    }
}
