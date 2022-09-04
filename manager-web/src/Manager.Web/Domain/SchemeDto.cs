namespace Manager.Web.Domain;

public sealed class SchemeDto : AuditDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public SchemeDataDto[] SchemeData { get; init; } = Array.Empty<SchemeDataDto>();
}
