using System.ComponentModel.DataAnnotations;

namespace Manager.Web.Pages.Forms;

public sealed class CreateSchemeForm
{
    [Required]
    [StringLength(40, ErrorMessage = "Title is too long")]
    public string Title { get; set; }
}
