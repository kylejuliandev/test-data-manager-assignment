namespace Manager.Web.Domain;

public abstract class AuditDto
{
    public string CreatedByUsername { get; init; } = default!;

    public DateTime CreatedOn { get; init; }

    public string ModifiedByUsername { get; init; } = default!;

    public DateTime ModifiedOn { get; init; }
}