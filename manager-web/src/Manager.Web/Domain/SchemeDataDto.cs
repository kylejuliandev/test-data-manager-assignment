namespace Manager.Web.Domain;

/// <summary>
/// Domain representation of the Scheme data
/// </summary>
public sealed class SchemeDataDto : AuditDto
{
    /// <summary>
    /// Unique identifier for the Scheme data
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The key of the Scheme, typically found in the format a_b_c (all lowercase)
    /// </summary>
    public string Key { get; init; } = default!;

    /// <summary>
    /// The value of the scheme data, any text field
    /// </summary>
    public string Value { get; init; } = default!;
}
