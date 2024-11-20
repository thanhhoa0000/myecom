using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using LoginRequest = MyEcom.Services.Identity.API.Models.Dtos.LoginRequest;

namespace MyEcom.Services.Identity.API.Endpoints;

// TODO: Customize Exception
public class IdentityEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = app
            .MapGroup("api/v{version:apiVersion}/auth")
            .WithApiVersionSet();

        group.MapPost("/assign_role", AssignRole);
        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
    }

    public static async Task<Results<Ok<string>, NotFound<string>, BadRequest<string>>> AssignRole(
        IDbContextFactory<AuthContext> dbContextFactory,
        string email,
        string role,
        [FromServices] UserManager<IdentityUser<Guid>> userManager,
        [FromServices] RoleManager<IdentityRole<Guid>> roleManager)
    {
        try
        {
            if (!Enum.TryParse(typeof(AppRoles), role, true, out _))
            {
                return TypedResults.BadRequest("Invalid role!");
            }

            await using var context = await dbContextFactory.CreateDbContextAsync();
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);

            if (user is not null)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    
                }

                var addRoleResult = await userManager.AddToRoleAsync(user, role);

                if (addRoleResult.Succeeded)
                {
                    return TypedResults.Ok("Role assigned!");
                }
                
                return TypedResults.BadRequest($"Error encountered when assigning role: {addRoleResult.Errors}");
            }
            
            return TypedResults.NotFound($"User with email {email} not found!");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Ok<LoginResponse>, NotFound<string>, BadRequest<string>>> Login(
        IDbContextFactory<AuthContext> dbContextFactory,
        ITokenProvider tokenProvider,
        [FromServices] UserManager<IdentityUser<Guid>> userManager,
        [FromBody] LoginRequest request,
        IMapper mapper)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
            {
                return TypedResults.NotFound($"User with username {request.UserName} not found!");
            }
            bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, request.Password);
            if (isPasswordCorrect == false)
            {
                return TypedResults.BadRequest("Invalid username or password!");
            }
            
            var roles = Enum.GetValues(typeof(AppRoles)).Cast<AppRoles>();;
            var token = tokenProvider.CreateToken(user, roles);
            
            var userDto = mapper.Map<AppUserDto>(user);

            LoginResponse response = new LoginResponse()
            {
                User = userDto,
                Token = token
            };
            
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    public static async Task<Results<Ok<AppUserDto>, BadRequest<string>>> Register(
        IDbContextFactory<AuthContext> dbContextFactory,
        [FromServices] UserManager<IdentityUser<Guid>> userManager,
        [FromBody] RegistrationRequest request,
        IMapper mapper)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var newUser = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            
            var createUserResult = await userManager.CreateAsync(newUser, request.Password);

            if (createUserResult.Succeeded)
            {
                return TypedResults.Ok(mapper.Map<AppUserDto>(newUser));
            }
            
            return TypedResults.BadRequest(
                $"Error encountered during registration: {createUserResult.Errors.FirstOrDefault()!.Description}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}