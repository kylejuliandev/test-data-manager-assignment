using Microsoft.AspNetCore.Identity;

namespace Manager.Web;

public static class WebApplicationExtensions
{
    public const string ADMIN_ROLE = "admin";

    public const string USER_ROLE = "user";

    /// <summary>
    /// Sets up default roles
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task CreateDefaultAuthorizationRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(ADMIN_ROLE))
            await roleManager.CreateAsync(new IdentityRole(ADMIN_ROLE));

        if (!await roleManager.RoleExistsAsync(USER_ROLE))
            await roleManager.CreateAsync(new IdentityRole(USER_ROLE));
    }
}
