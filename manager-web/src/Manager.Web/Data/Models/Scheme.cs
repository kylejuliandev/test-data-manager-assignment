namespace Manager.Web.Data.Models;

/// <summary>
/// A scheme is a grouping of Scheme data
/// </summary>
public class Scheme : AuditableEntity
{
    /// <summary>
    /// Unique id of the Scheme
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Scheme title that idenifies the scheme
    /// /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The associated Scheme Data, Entity Framework Core navigation property
    /// </summary>
    public List<SchemeData> Data { get; set; }
}
