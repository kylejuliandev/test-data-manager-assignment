namespace Manager.Web.Data.Models;

public class Scheme : AuditableEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public List<SchemeData> Data { get; set; }
}
