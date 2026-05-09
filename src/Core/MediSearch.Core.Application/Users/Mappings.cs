using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Users.Commands.RegisterUser;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Users;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<User, UserDto>()
            .Map(dest => dest.FirstName, src => src.FullName.FirstName)
            .Map(dest => dest.LastName, src => src.FullName.LastName)
            .Map(dest => dest.Province, src => src.Location.Province)
            .Map(dest => dest.Municipality, src => src.Location.Municipality)
            .Map(dest => dest.Address, src => src.Location.Address);

        config
            .NewConfig<RegisterUserCommand, RegisterExternalUserDto>()
            .Ignore(dest => dest.IsActive);
    }
}
