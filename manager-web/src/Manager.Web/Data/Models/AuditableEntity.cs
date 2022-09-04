using Microsoft.AspNetCore.Identity;

namespace Manager.Web.Data.Models;

public class AuditableEntity
{
    public string CreatedById { get; set; }

    public IdentityUser CreatedBy { get; set; }

    public string ModifiedById { get; set; }

    public IdentityUser ModifiedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }
}
