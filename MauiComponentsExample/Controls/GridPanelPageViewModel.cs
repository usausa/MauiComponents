namespace MauiComponentsExample.Controls;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class GridPanelPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial int StaggeredColumns { get; set; } = 3;

    [ObservableProperty]
    public partial double StaggeredSpacing { get; set; } = 8;

    [ObservableProperty]
    public partial int WrapColumns { get; set; } = 3;

    [ObservableProperty]
    public partial double WrapSpacing { get; set; } = 8;

    [ObservableProperty]
    public partial bool AutoRowHeight { get; set; }

    [ObservableProperty]
    public partial double FixedRowHeight { get; set; } = 72;

    [ObservableProperty]
    public partial double RowHeight { get; set; } = 72;

    public GridPanelPageViewModel()
    {
        SubscribeAutoRowHeight(_ => UpdateRowHeight());
        SubscribeFixedRowHeight(_ => UpdateRowHeight());
    }

    private void UpdateRowHeight()
    {
        RowHeight = AutoRowHeight ? Double.NaN : FixedRowHeight;
    }
}
