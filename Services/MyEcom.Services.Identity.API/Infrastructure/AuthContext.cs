namespace MyEcom.Services.Identity.API.Infrastructure;

public class AuthContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }
    
    public DbSet<AppUser> AppUsers { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}