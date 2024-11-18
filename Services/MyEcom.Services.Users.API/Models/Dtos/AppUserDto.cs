using Microsoft.AspNetCore.Identity;

namespace MyEcom.Services.Users.API.Models.Dtos;

public class AppUserDto : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}