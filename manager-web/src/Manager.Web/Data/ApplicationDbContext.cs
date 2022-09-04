using Manager.Web.Data.Configuration;
using Manager.Web.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manager.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbSet<Scheme> Schemes { get; set; }
    
    public DbSet<SchemeData> SchemeData { get; set; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new IdentityRoleConfiguration());
        builder.ApplyConfiguration(new SchemeConfiguration());
        builder.ApplyConfiguration(new SchemeDataConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && (e.State is EntityState.Added || e.State is EntityState.Modified));

        if (entries.Any())
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is null || context.User is null)
                throw new NotAuthorizedException();

            var userIdClaim = context.User.FindFirst(r => r.Type is ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                throw new NotAuthorizedException();

            var user = await Users.FindAsync(new[] { userIdClaim.Value }, cancellationToken);

            if (user is null)
                throw new NotAuthorizedException();

            foreach (var change in entries)
            {
                var auditEntry = (AuditableEntity)change.Entity;

                if (change.State is EntityState.Added)
                {
                    auditEntry.CreatedAt = DateTime.UtcNow;
                    auditEntry.CreatedById = user.Id;
                }
                else
                {
                    Entry(auditEntry).Property(p => p.CreatedById).IsModified = false;
                    Entry(auditEntry).Property(p => p.CreatedAt).IsModified = false;
                }

                auditEntry.ModifiedAt = DateTime.UtcNow;
                auditEntry.ModifiedById = user.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
