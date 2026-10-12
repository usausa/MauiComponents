namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public sealed partial class ShimmerPageViewModel : ExtendViewModelBase
{
    public IReadOnlyList<string> Articles { get; } =
    [
        "Weekend market opens in the park",
        "New cycling route along the river",
        "Library extends its opening hours"
    ];

    [ObservableProperty]
    public partial bool IsActive { get; set; } = true;

    [ObservableProperty]
    public partial double WaveWidth { get; set; } = 200;

    [ObservableProperty]
    public partial double Duration { get; set; } = 1000;

    [ObservableProperty]
    public partial int FillColorIndex { get; set; }

    [ObservableProperty]
    public partial int WaveColorIndex { get; set; }

    [ObservableProperty]
    public partial int SurfaceColorIndex { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsLoaded { get; set; } = true;

    public ICommand ReloadCommand { get; }

    public ShimmerPageViewModel()
    {
        ReloadCommand = MakeAsyncCommand(async () =>
        {
            IsLoaded = false;
            IsLoading = true;
            await Task.Delay(3000);
            IsLoading = false;
            IsLoaded = true;
        });
    }
}
