namespace Manager.Web.Data.Models;

/// <summary>
/// Scheme data is the single piece of test configuration. Test configuration is set as key/value pairs.
/// </summary>
public class SchemeData : AuditableEntity
{
    /// <summary>
    /// The unique identifier for the Scheme data
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The key of the Scheme Data, uniquely identifies the test configuration. Typically in the format a_b_c
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The value of the Scheme Data, any text field
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The scheme that this Scheme Data belongs to. Foreign key
    /// </summary>
    public Guid SchemeId { get; set; }

    /// <summary>
    /// The scheme that this Scheme Data belongs to, Entity Framework Core navigation property
    /// </summary>
    public Scheme Scheme { get; set; }
}
