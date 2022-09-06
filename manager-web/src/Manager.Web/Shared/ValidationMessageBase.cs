using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Manager.Web.Shared;

/// <summary>
/// Reusable component for Blazor to render the Validation messages from a Edit Context for a specific field
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ValidationMessageBase<TValue> : ComponentBase, IDisposable
{
    private FieldIdentifier _fieldIdentifier;

    [CascadingParameter] private EditContext EditContext { get; set; }
    [Parameter] public Expression<Func<TValue>> For { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }

    protected IEnumerable<string> ValidationMessages => EditContext.GetValidationMessages(_fieldIdentifier);

    protected override void OnInitialized()
    {
        _fieldIdentifier = FieldIdentifier.Create(For);
        EditContext.OnValidationStateChanged += HandleValidationStateChanged;
    }

    private void HandleValidationStateChanged(object o, ValidationStateChangedEventArgs args) => StateHasChanged();

    public void Dispose()
    {
        EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
    }
}