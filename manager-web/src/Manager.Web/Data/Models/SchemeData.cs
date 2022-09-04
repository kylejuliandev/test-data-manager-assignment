namespace Manager.Web.Data.Models;

public class SchemeData : AuditableEntity
{
    public Guid Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public Guid SchemeId { get; set; }

    public Scheme Scheme { get; set; }
}
