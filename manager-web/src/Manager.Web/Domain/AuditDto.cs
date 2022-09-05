namespace Manager.Web.Domain;

/// <summary>
/// A domain based representation of the Audit data, abstract is it will not be found without a derived type
/// </summary>
public abstract class AuditDto
{
    /// <summary>
    /// The user who originally created the entity
    /// </summary>
    public string CreatedByUsername { get; init; } = default!;

    /// <summary>
    /// The datetime when the entity was created
    /// </summary>
    public DateTime CreatedOn { get; init; }

    /// <summary>
    /// The user who last modified the entity
    /// </summary>
    public string ModifiedByUsername { get; init; } = default!;

    /// <summary>
    /// The datetime when the entity was last changed
    /// </summary>
    public DateTime ModifiedOn { get; init; }
}