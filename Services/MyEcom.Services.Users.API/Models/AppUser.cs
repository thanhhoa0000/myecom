using Microsoft.AspNetCore.Identity;

namespace MyEcom.Services.Users.API.Models;

public class AppUser : IdentityUser<Guid>
{
    [Required, MinLength(2), MaxLength(50)]
    public required string FirstName { get; set; }
    [Required, MinLength(2), MaxLength(80)]
    public required string LastName { get; set; }
}