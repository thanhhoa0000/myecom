namespace MyEcom.Services.Users.API;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<AppUser, AppUserDto>().ReverseMap();
    }
}