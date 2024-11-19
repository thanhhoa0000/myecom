namespace MyEcom.Services.Identity.API.Models.Dtos;

public class LoginResponse
{
    public AppUserDto User { get; set; }
    public string Token { get; set; }
}