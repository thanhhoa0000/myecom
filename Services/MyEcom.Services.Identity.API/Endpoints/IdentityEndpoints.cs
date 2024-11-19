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
        UserManager<IdentityUser<Guid>> userManager,
        RoleManager<IdentityRole> roleManager)
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
                    await roleManager.CreateAsync(new IdentityRole(role));
                    
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
        UserManager<IdentityUser<Guid>> userManager,
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
    
    public static async Task<Results<Created, BadRequest<string>>> Register(
        IDbContextFactory<AuthContext> dbContextFactory,
        UserManager<IdentityUser<Guid>> userManager,
        [FromBody] RegisterRequest request,
        IMapper mapper)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}