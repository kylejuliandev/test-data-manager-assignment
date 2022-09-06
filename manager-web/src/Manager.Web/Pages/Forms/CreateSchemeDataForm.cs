using System.ComponentModel.DataAnnotations;

namespace Manager.Web.Pages.Forms;

/// <summary>
/// Form to represent the necessary attributes required by the user when creating a Scheme data item
/// </summary>
public sealed class CreateSchemeDataForm
{
    /// <summary>
    /// The key of the data item (in the form a_b_c all lowercase)
    /// </summary>
    [Required]
    [StringLength(120, ErrorMessage = "Key is too long")]
    [RegularExpression("([a-z]+_*)*", ErrorMessage = "The key must in the style key_name_name")]
    public string Key { get; set; }

    /// <summary>
    /// The vale of the data item (any text)
    /// </summary>
    [Required]
    public string Value { get; set; }
}
