using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Manager.Web.Areas.Identity;
using Manager.Web.Data;
using Manager.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

// Set up local Identity server
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Policies to the Identity server
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ListSchemes", policy => policy.RequireRole("user","admin","superuser"));
    options.AddPolicy("CreateScheme", policy => policy.RequireRole("admin", "superuser"));
    options.AddPolicy("EditScheme", policy => policy.RequireRole("admin", "superuser"));
    options.AddPolicy("RemoveScheme", policy => policy.RequireRole("admin", "superuser"));
    options.AddPolicy("CreateSchemeData", policy => policy.RequireRole("admin", "superuser"));
    options.AddPolicy("RemoveSchemeData", policy => policy.RequireRole("admin", "superuser"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add the Scheme Service so it can be injected in to the frontend components
builder.Services.AddScoped<SchemeService>();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // We do not want to show stacktraces to the end user, as this makes the application insecure
    app.UseExceptionHandler("/Error");

    // Adds the strict transport security header
    // We only want to support HTTPS for security reasons (as we expose a login and sign up page)
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Seed the necessary data in the database
await app.SeedUsersAsync();

await app.RunAsync();
