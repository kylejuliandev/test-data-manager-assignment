namespace Manager.Web.Domain;

/// <summary>
/// Domain representation of the Scheme and the associated Scheme data
/// </summary>
public sealed class SchemeDto : AuditDto
{
    /// <summary>
    /// Unique identifier of the Scheme
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Scheme title
    /// </summary>
    public string Title { get; init; } = default!;

    /// <summary>
    /// The associated scheme data
    /// </summary>
    public SchemeDataDto[] SchemeData { get; init; } = Array.Empty<SchemeDataDto>();
}
