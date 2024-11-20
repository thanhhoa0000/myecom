namespace MyEcom.Services.Identity.API.Models.Dtos;

public class RegistrationRequest
{
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    [MaxLength(12)]
    public string PhoneNumber { get; set; }
    [MinLength(2), MaxLength(50)]
    public string FirstName { get; set; }
    [MinLength(2), MaxLength(80)]
    public string LastName { get; set; }
    public AppRoles Role { get; set; } = AppRoles.Customer;
}