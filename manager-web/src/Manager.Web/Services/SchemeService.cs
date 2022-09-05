using Manager.Web.Data;
using Manager.Web.Data.Models;
using Manager.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Manager.Web.Services;

public sealed class SchemeService
{
    private readonly ILogger<SchemeService> _logger;
    private readonly ApplicationDbContext _database;

    public SchemeService(ILogger<SchemeService> logger, ApplicationDbContext database)
    {
        _logger = logger;
        _database = database;
    }

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
            .OrderByDescending(s => s.CreatedAt)
            .Skip(page * pageSize)
            .Take(pageSize + 1)
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
        var scheme = await GetScheme(schemeId);

        if (scheme is null)
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Scheme not found") });

        return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
    }

    public async Task<ManagedResponse<SchemeDto>> CreateSchemeAsync(string title)
    {
        try
        {
            var schemeId = Guid.NewGuid();
            var scheme = new Scheme
            {
                Id = schemeId,
                Title = title
            };

            await _database.AddAsync(scheme);
            await _database.SaveChangesAsync();

            scheme = await GetScheme(schemeId);

            return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme!), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create Scheme in database");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to save Scheme to the server") });
        }
    }

    public async Task<ManagedResponse<SchemeDto>> UpdateSchemeAsync(Guid schemeId, string title)
    {
        var scheme = await GetScheme(schemeId);

        if (scheme is null)
        {
            _logger.LogWarning("Scheme not found, cannot update the scheme");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("You cannot update this scheme") });
        }

        if (scheme.Title == title)
        {
            return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
        }

        scheme.Title = title;

        await _database.SaveChangesAsync();

        return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
    }

    public async Task<ManagedResponse<SchemeDto>> CreateSchemeDataAsync(Guid schemeId, string key, string value)
    {
        var scheme = await GetScheme(schemeId);

        if (scheme is null)
        {
            _logger.LogWarning("Scheme not found, cannot add scheme data");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("You cannot add scheme data to this scheme") });
        }

        var dataKeyExists = await _database.SchemeData.FirstOrDefaultAsync(sd => EF.Functions.Collate(sd.Key, "NOCASE") == key);
        if (dataKeyExists is not null)
        {
            _logger.LogWarning("Cannot add duplicate test data key");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("The key field must be unique") });
        }

        var schemeDataId = Guid.NewGuid();
        var schemeData = new SchemeData
        {
            Id = schemeDataId,
            Key = key,
            Value = value,
            Scheme = scheme
        };

        await _database.SchemeData.AddAsync(schemeData);

        await _database.SaveChangesAsync();

        return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
    }

    public async Task<ManagedResponse<bool>> RemoveSchemeDataAsync(Guid schemeId, Guid schemeDataId)
    {
        var schemeData = await _database.SchemeData.FirstOrDefaultAsync(sd => sd.SchemeId == schemeId && sd.Id == schemeDataId);

        if (schemeData is null)
        {
            _logger.LogWarning("Scheme Data not found, cannot remove the scheme data");

            return new ManagedResponse<bool>(false, new[] { new ManagedError("You cannot remove this scheme data") });
        }

        _database.Remove(schemeData);
        await _database.SaveChangesAsync();

        return new ManagedResponse<bool>(true);
    }

    public async Task<ManagedResponse<bool>> RemoveSchemeAsync(Guid schemeId)
    {
        var scheme = await GetScheme(schemeId);

        if (scheme is null)
        {
            _logger.LogWarning("Scheme not found, cannot remove the scheme");

            return new ManagedResponse<bool>(false, new[] { new ManagedError("You cannot remove this scheme") });
        }

        _database.Schemes.Remove(scheme);
        await _database.SaveChangesAsync();

        return new ManagedResponse<bool>(true);
    }

    private async Task<Scheme?> GetScheme(Guid schemeId) => await _database.Schemes
        .Include(s => s.CreatedBy)
        .Include(s => s.ModifiedBy)
        .Include(s => s.Data)
        .FirstOrDefaultAsync(s => s.Id == schemeId);

    private static SchemeDto MapSchemeToSchemeDto(Scheme scheme) => new()
    {
        Id = scheme!.Id,
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
    };
}
