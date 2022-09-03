using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Manager.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    // Do not change
    const string USER_ROLE_ID = "9ffe1c8e-6dd6-4d1f-8e5a-93911e41cc90";
    const string ADMIN_ROLE_ID = "bd2b7bc6-fa36-4988-9563-7ff609cf794c";
    const string SUPERADMIN_ROLE_ID = "02171634-f2c9-4054-966f-675702641552";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = USER_ROLE_ID, Name = "user", NormalizedName = "USER" });
        builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = ADMIN_ROLE_ID, Name = "admin", NormalizedName = "ADMIN" });
        builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = SUPERADMIN_ROLE_ID, Name = "superadmin", NormalizedName = "SUPERADMIN" });
    }
}
