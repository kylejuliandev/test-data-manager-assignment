using Microsoft.AspNetCore.Identity;

namespace Manager.Web.Data;

public static class ApplicationDbInitializer
{
    const string ADMIN_ID = "69dee697-c3f3-46f1-87a3-867a392902be";

    /// <summary>
    /// Create default system users
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task SeedUsersAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (await userManager.FindByNameAsync("superadmin") is null)
        {
            var superAdmin = new IdentityUser
            {
                Id = ADMIN_ID,
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                Email = "superadmin@testdatamanager.dev",
                NormalizedEmail = "superadmin@testdatamanager.dev",
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            };

            var result = await userManager.CreateAsync(superAdmin, app.Configuration["Users:superadmin:Password"]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "superadmin");
            }
        }
    }
}
