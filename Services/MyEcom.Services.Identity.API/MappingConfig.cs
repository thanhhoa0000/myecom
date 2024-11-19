namespace MyEcom.Services.Identity.API;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<AppUser, AppUserDto>().ReverseMap();
    }
}