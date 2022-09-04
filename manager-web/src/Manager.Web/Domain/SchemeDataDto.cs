namespace Manager.Web.Domain;

public sealed class SchemeDataDto : AuditDto
{
    public Guid Id { get; init; }

    public string Key { get; init; } = default!;

    public string Value { get; init; } = default!;
}
