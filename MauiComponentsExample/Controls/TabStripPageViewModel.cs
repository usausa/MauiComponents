namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class TabStripPageViewModel : ExtendViewModelBase
{
    private static readonly string[] AllTabs = ["Home", "Popular", "Sports", "Music", "Movies", "Science", "Travel", "Food", "Games", "Books"];

    [ObservableProperty]
    public partial IReadOnlyList<string> Tabs { get; set; } = AllTabs[..6];

    [ObservableProperty]
    public partial int SelectedIndex { get; set; }

    [ObservableProperty]
    public partial string Current { get; set; } = AllTabs[0];

    // Swipe of the pages from the selected page (-1 = previous page, 1 = next page)
    [ObservableProperty]
    public partial double Swipe { get; set; }

    // NaN while not swiping
    [ObservableProperty]
    public partial double PagePosition { get; set; } = Double.NaN;

    [ObservableProperty]
    public partial int IndicatorColorIndex { get; set; }

    [ObservableProperty]
    public partial double IndicatorHeight { get; set; } = 3;

    [ObservableProperty]
    public partial double IndicatorInset { get; set; } = 12;

    public ICommand SwipeCompletedCommand { get; }

    public ICommand PreviousCommand { get; }

    public ICommand NextCommand { get; }

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public TabStripPageViewModel()
    {
        SwipeCompletedCommand = MakeDelegateCommand(() =>
        {
            var target = Math.Clamp((int)Math.Round(SelectedIndex + Swipe), 0, Tabs.Count - 1);
            PagePosition = target;
            SelectedIndex = target;
            Swipe = 0;
            PagePosition = Double.NaN;
        });
        PreviousCommand = MakeDelegateCommand(() => SelectedIndex = Math.Max(SelectedIndex - 1, 0));
        NextCommand = MakeDelegateCommand(() => SelectedIndex = Math.Min(SelectedIndex + 1, Tabs.Count - 1));
        AddCommand = MakeDelegateCommand(() => UpdateTabs(Math.Min(Tabs.Count + 1, AllTabs.Length)));
        RemoveCommand = MakeDelegateCommand(() => UpdateTabs(Math.Max(Tabs.Count - 1, 1)));

        SubscribeSwipe(swipe =>
        {
            if (swipe != 0)
            {
                PagePosition = Math.Clamp(SelectedIndex + swipe, 0, Tabs.Count - 1);
            }
        });
        SubscribeSelectedIndex(index => Current = Tabs[index]);
    }

    private void UpdateTabs(int count)
    {
        Tabs = AllTabs[..count];
        if (SelectedIndex >= count)
        {
            SelectedIndex = count - 1;
        }
    }
}
