using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;

namespace WebApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "5736062e-cf47-4323-9de9-ffbba4161963", Name = "Admin", ConcurrencyStamp = "1",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "720e8e4d-1b1e-4c80-8448-841bfd0a1355", Name = "User", ConcurrencyStamp = "2",
                NormalizedName = "USER"
            }
        );
    }

    public DbSet<Pizza>? Pizzas { get; set; }
}