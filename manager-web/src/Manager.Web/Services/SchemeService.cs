using Manager.Web.Data;
using Manager.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Manager.Web.Services;

public sealed class SchemeService
{
    private readonly ApplicationDbContext _database;

    public SchemeService(ApplicationDbContext database)
    {
        _database = database;
    }

    [Authorize(Policy = "ListSchemes")]
    public async Task<ManagedPaginatedResponse<ListSchemeDto>> GetSchemesAsync(int page, int pageSize)
    {
        if (page < 0 || pageSize < 0)
        {
            return new ManagedPaginatedResponse<ListSchemeDto>(errors: new[]
            {
                new ManagedError("You must specify a valid page and pageSize")
            });
        }

        var schemes = await _database.Schemes
            .Include(s => s.CreatedBy)
            .Include(s => s.ModifiedBy)
            .Skip(page * pageSize)
            .Take(pageSize + 1)
            .OrderByDescending(s => s.CreatedAt)
            .ToArrayAsync();

        var schemeDtos = schemes
            .Take(pageSize)
            .Select(s => new ListSchemeDto
            {
                Id = s.Id, 
                Title = s.Title, 
                CreatedByUsername = s.CreatedBy.UserName, 
                CreatedOn = s.CreatedAt,
                ModifiedByUsername = s.ModifiedBy.UserName, 
                ModifiedOn = s.ModifiedAt
            })
            .ToArray();

        return new ManagedPaginatedResponse<ListSchemeDto>(schemeDtos, schemes.Length > pageSize);
    }

    public async Task<ManagedResponse<SchemeDto>> GetSchemeAsync(Guid schemeId)
    {
        var scheme = await _database.Schemes
            .Include(s => s.CreatedBy)
            .Include(s => s.ModifiedBy)
            .Include(s => s.Data)
            .FirstOrDefaultAsync(s => s.Id == schemeId);

        if (scheme is null)
        {
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Scheme not found") });
        }

        return new ManagedResponse<SchemeDto>(new SchemeDto
        {
            Id = scheme.Id,
            Title = scheme.Title,
            SchemeData = scheme.Data
                .Select(d => new SchemeDataDto
                {
                    Id = d.Id,
                    Key = d.Key,
                    Value = d.Value
                })
                .ToArray(),
            CreatedByUsername = scheme.CreatedBy.UserName,
            CreatedOn = scheme.CreatedAt,
            ModifiedByUsername = scheme.ModifiedBy.UserName,
            ModifiedOn = scheme.ModifiedAt
        }, null);
    }
}
