using Manager.Web.Data;
using Manager.Web.Data.Models;
using Manager.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Manager.Web.Services;

/// <summary>
/// Domain based Scheme service that abstracts out database interactions from the UI. Intended to be reused in other areas of the platform, like in a HTTP REST API web application.
/// </summary>
public sealed class SchemeService
{
    private readonly ILogger<SchemeService> _logger;
    private readonly ApplicationDbContext _database;

    public SchemeService(ILogger<SchemeService> logger, ApplicationDbContext database)
    {
        _logger = logger;
        _database = database;
    }

    /// <summary>
    /// Retrieve a paginated set of Schemes from the database
    /// </summary>
    /// <param name="page">The page you are wanting to access</param>
    /// <param name="pageSize">The number of items to include in the set</param>
    /// <returns></returns>
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

    /// <summary>
    /// Retrieve a specific scheme from the database
    /// </summary>
    /// <param name="schemeId"></param>
    /// <returns></returns>
    public async Task<ManagedResponse<SchemeDto>> GetSchemeAsync(Guid schemeId)
    {
        var scheme = await GetScheme(schemeId);

        if (scheme is null)
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Scheme not found") });

        return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
    }

    /// <summary>
    /// Create a scheme with the specified title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
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

            _database.Add(scheme);
            await _database.SaveChangesAsync();

            scheme = await GetScheme(schemeId);

            return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme!), null);
        }
        catch (NotAuthorizedException)
        {
            _logger.LogError("An unauthorized user attempted to create a Scheme");

            // We return a generic error message back to the user
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to save Scheme to the database") });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create Scheme in database");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to save Scheme to the database") });
        }
    }

    /// <summary>
    /// Update a scheme with the specified title
    /// </summary>
    /// <param name="schemeId"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<ManagedResponse<SchemeDto>> UpdateSchemeAsync(Guid schemeId, string title)
    {
        try
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
        catch (NotAuthorizedException)
        {
            _logger.LogError("An unauthorized user attempted to update a Scheme");

            // We return a generic error message back to the user
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to update Scheme in the database") });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update Scheme in database");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to update Scheme in the database") });
        }
    }

    /// <summary>
    /// Create scheme data with the specified key and value pair
    /// </summary>
    /// <param name="schemeId"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<ManagedResponse<SchemeDto>> CreateSchemeDataAsync(Guid schemeId, string key, string value)
    {
        try
        {
            var scheme = await GetScheme(schemeId);

            if (scheme is null)
            {
                _logger.LogWarning("Scheme not found, cannot add scheme data");

                return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("You cannot add scheme data to this scheme") });
            }

            var dataKeyExists = await _database.SchemeData.FirstOrDefaultAsync(sd => sd.Key == key);
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

            _database.SchemeData.Add(schemeData);
            await _database.SaveChangesAsync();

            return new ManagedResponse<SchemeDto>(MapSchemeToSchemeDto(scheme), null);
        }
        catch (NotAuthorizedException)
        {
            _logger.LogError("An unauthorized user attempted to create a Scheme data");

            // We return a generic error message back to the user
            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to create Scheme data in database") });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create Scheme data in database");

            return new ManagedResponse<SchemeDto>(null, new[] { new ManagedError("Unable to create Scheme data in database") });
        }
    }

    /// <summary>
    /// Remove the specified scheme data from the database
    /// </summary>
    /// <param name="schemeId"></param>
    /// <param name="schemeDataId"></param>
    /// <returns></returns>
    public async Task<ManagedResponse<bool>> RemoveSchemeDataAsync(Guid schemeId, Guid schemeDataId)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to remove scheme data from database");

            return new ManagedResponse<bool>(false, new[] { new ManagedError("Unable to remove Scheme data from the database") });
        }
    }

    /// <summary>
    /// Remove the specified scheme from the database
    /// </summary>
    /// <param name="schemeId"></param>
    /// <returns></returns>
    public async Task<ManagedResponse<bool>> RemoveSchemeAsync(Guid schemeId)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to remove scheme from database");

            return new ManagedResponse<bool>(false, new[] { new ManagedError("Unable to remmove Scheme from the database") });
        }
    }

    /// <summary>
    /// Retrieve a Scheme and include the relationships
    /// </summary>
    /// <param name="schemeId"></param>
    /// <returns></returns>
    private async Task<Scheme?> GetScheme(Guid schemeId) => await _database.Schemes
        .Include(s => s.CreatedBy)
        .Include(s => s.ModifiedBy)
        .Include(s => s.Data)
        .FirstOrDefaultAsync(s => s.Id == schemeId);

    /// <summary>
    /// Map a entity Scheme to a domain based Scheme
    /// </summary>
    /// <param name="scheme"></param>
    /// <returns></returns>
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
