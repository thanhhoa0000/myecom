namespace MyEcom.Services.Identity.API.Services.IServices;

public interface ITokenProvider
{
    string CreateToken(AppUser user, IEnumerable<AppRoles> roles);
}