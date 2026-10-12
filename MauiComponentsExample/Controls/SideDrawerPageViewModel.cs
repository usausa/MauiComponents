namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public sealed partial class SideDrawerPageViewModel : ExtendViewModelBase
{
    private static readonly string[] MenuNames = ["Inbox", "Starred", "Sent", "Drafts", "Archive", "Trash"];

    public IReadOnlyList<SampleAction> MenuItems { get; }

    [ObservableProperty]
    public partial bool IsOpen { get; set; }

    [ObservableProperty]
    public partial bool EdgeSwipeEnabled { get; set; } = true;

    [ObservableProperty]
    public partial double DrawerWidth { get; set; } = 280;

    [ObservableProperty]
    public partial double Duration { get; set; } = 150;

    [ObservableProperty]
    public partial double EdgeWidth { get; set; } = 24;

    [ObservableProperty]
    public partial int DrawerBackgroundColorIndex { get; set; }

    [ObservableProperty]
    public partial string Current { get; set; } = "Inbox";

    public ICommand OpenCommand { get; }

    public ICommand SelectCommand { get; }

    public SideDrawerPageViewModel()
    {
        OpenCommand = MakeDelegateCommand(() => IsOpen = true);
        SelectCommand = MakeDelegateCommand<string>(x =>
        {
            Current = x;
            IsOpen = false;
        });
        MenuItems = [.. MenuNames.Select(x => new SampleAction(x, SelectCommand))];
    }
}
