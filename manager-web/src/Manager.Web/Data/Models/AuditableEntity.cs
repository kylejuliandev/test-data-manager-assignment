using Microsoft.AspNetCore.Identity;

namespace Manager.Web.Data.Models;

/// <summary>
/// Audit meta data. Abstract as it is intended to be implemented by derived database entities
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    /// User who created the Audit item, does not change after entity creation. Foreign key
    /// </summary>
    public string CreatedById { get; set; }

    /// <summary>
    /// User who created the Audit item, Entity Framework core navigation property, does not change after entity creation
    /// </summary>
    public IdentityUser CreatedBy { get; set; }

    /// <summary>
    /// User who last modified the Audit item. Foreign key
    /// </summary>
    public string ModifiedById { get; set; }

    /// <summary>
    /// User who last modified the Audit item, Entity Framework core navigation property
    /// </summary>
    public IdentityUser ModifiedBy { get; set; }

    /// <summary>
    /// When the Audit item was created, does not change after entity creation
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the Audit item was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}
