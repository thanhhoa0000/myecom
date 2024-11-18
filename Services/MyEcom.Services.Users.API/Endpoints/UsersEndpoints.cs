using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace MyEcom.Services.Users.API.Endpoints;

// TODO: Customize Exception
public class UsersEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = app
            .MapGroup("api/v{version:apiVersion}/users")
            .WithApiVersionSet();

        group.MapGet("/", GetAllUsers);
        group.MapGet("/{id:guid}", GetUserById);
        group.MapPost("/", AddUser);
        group.MapPut("/", UpdateUser);
        group.MapDelete("/{id:guid}", DeleteUser);
    }

    public static async Task<Results<Ok<IEnumerable<AppUserDto>>, BadRequest<string>>> GetAllUsers(
        IDbContextFactory<UserContext> dbContextFactory,
        IMapper mapper)
    {
        try
        {
            var context = dbContextFactory.CreateDbContext();
            var users = await context.AppUsers.ToListAsync();

            return TypedResults.Ok(mapper.Map<IEnumerable<AppUserDto>>(users));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Ok<AppUserDto>, NotFound<string>, BadRequest<string>>> GetUserById(
        IDbContextFactory<UserContext> dbContextFactory,
        IMapper mapper,
        Guid id)
    {
        try
        {
            var context = dbContextFactory.CreateDbContext();
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return TypedResults.NotFound($"User with id \"{id}\" not found!");
            }

            return TypedResults.Ok(mapper.Map<AppUserDto>(user));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Created, BadRequest<string>>> AddUser(
        IDbContextFactory<UserContext> dbContextFactory,
        IMapper mapper,
        [FromBody] AppUserDto userDto)
    {
        try
        {
            var context = dbContextFactory.CreateDbContext();
            var user = mapper.Map<AppUser>(userDto);

            context.AppUsers.Add(user);
            await context.SaveChangesAsync();

            return TypedResults.Created($"/users/{userDto.Id}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> UpdateUser(
        IDbContextFactory<UserContext> dbContextFactory,
        IMapper mapper,
        [FromBody] AppUserDto userDto)
    {
        try
        {
            var context = dbContextFactory.CreateDbContext();
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);

            if (user is null)
            {
                return TypedResults.NotFound($"User with id \"{userDto.Id}\" not found!");
            }

            var userEntry = context.Entry(user);
            userEntry.CurrentValues.SetValues(userDto);

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> DeleteUser(
        IDbContextFactory<UserContext> dbContextFactory,
        IMapper mapper,
        Guid id)
    {
        try
        {
            var context = dbContextFactory.CreateDbContext();
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return TypedResults.NotFound($"User with id \"{id}\" not found!");
            }

            context.AppUsers.Remove(user);
            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}


