using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.DevToys.Core;

namespace WindowSill.DevToys;

internal sealed partial class DevToysSillViewModel : ObservableObject
{
    [RelayCommand]
    private async Task FormatJsonAsync((WindowTextSelection currentSelection, Indentation indentation) value)
    {
        string formattedJson = JsonHelper.Format(value.currentSelection.SelectedText, value.indentation);

        if (!string.IsNullOrEmpty(formattedJson))
        {
            await TextInjector.InjectAsync(formattedJson, value.currentSelection);
        }
    }
}
