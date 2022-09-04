namespace Manager.Web.Domain;

public class ListSchemeDto : AuditDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;
}