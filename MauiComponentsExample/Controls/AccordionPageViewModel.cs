namespace MauiComponentsExample.Controls;

using MauiComponents.Controls;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public sealed partial class AccordionPageViewModel : ExtendViewModelBase
{
    public IReadOnlyList<AccordionExpandMode> ExpandModes { get; } = [AccordionExpandMode.MultipleOrNone, AccordionExpandMode.SingleOrNone];

    public IReadOnlyList<string> Icons { get; } = ["▾", "▼", "∨"];

    [ObservableProperty]
    public partial AccordionExpandMode ExpandMode { get; set; } = AccordionExpandMode.MultipleOrNone;

    [ObservableProperty]
    public partial string Icon { get; set; } = "▾";

    [ObservableProperty]
    public partial int IconColorIndex { get; set; }

    [ObservableProperty]
    public partial int LineColorIndex { get; set; }

    [ObservableProperty]
    public partial bool IsShippingExpanded { get; set; } = true;
}
