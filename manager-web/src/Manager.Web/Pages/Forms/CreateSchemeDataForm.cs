using System.ComponentModel.DataAnnotations;

namespace Manager.Web.Pages.Forms;

public class CreateSchemeDataForm
{
    [Required]
    [StringLength(120, ErrorMessage = "Key is too long")]
    [RegularExpression("([a-z]+_*)*", ErrorMessage = "The key must in the style key_name_name")]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }
}
