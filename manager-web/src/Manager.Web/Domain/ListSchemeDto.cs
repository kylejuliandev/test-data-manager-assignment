namespace Manager.Web.Domain;

/// <summary>
/// A specialised domain model that only contains the relevant information for a listing view
/// </summary>
public sealed class ListSchemeDto : AuditDto
{
    /// <summary>
    /// Id of the scheme
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Title of the scheme
    /// </summary>
    public string Title { get; init; } = default!;
}