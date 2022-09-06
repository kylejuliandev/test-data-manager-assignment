using System.ComponentModel.DataAnnotations;

namespace Manager.Web.Pages.Forms;

/// <summary>
/// Form that represents the necessary fields to create a Scheme
/// </summary>
public sealed class CreateSchemeForm
{
    /// <summary>
    /// Title of the scheme, maximum 40 characters in length
    /// </summary>
    [Required]
    [StringLength(40, ErrorMessage = "Title is too long")]
    public string Title { get; set; }
}
