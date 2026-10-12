namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using MauiComponentsExample.Dialogs;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public sealed record SampleCategory(string Icon, string Name)
{
    public string Text => $"{Icon} {Name}";
}

public sealed partial class ChipSegmentPageViewModel : ExtendViewModelBase
{
    public IReadOnlyList<string> Fruits { get; } = ["Apple", "Banana", "Cherry", "Grape", "Lemon", "Mango", "Orange", "Peach", "Pear", "Plum"];

    public IReadOnlyList<SampleCategory> Categories { get; } =
    [
        new("🍔", "Food"),
        new("🎵", "Music"),
        new("⚽", "Sports"),
        new("✈️", "Travel"),
        new("📚", "Books"),
        new("🎮", "Games")
    ];

    public IReadOnlyList<string> Periods { get; } = ["Day", "Week", "Month", "Year"];

    [ObservableProperty]
    public partial string? SelectedFruit { get; set; } = "Apple";

    [ObservableProperty]
    public partial SampleCategory? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial bool BoldItems { get; set; }

    [ObservableProperty]
    public partial double ChipTextSize { get; set; } = 16;

    [ObservableProperty]
    public partial double ChipCornerRadius { get; set; } = 18;

    [ObservableProperty]
    public partial double ChipStrokeThickness { get; set; } = 1;

    [ObservableProperty]
    public partial double Spacing { get; set; } = 8;

    [ObservableProperty]
    public partial int SelectedChipColorIndex { get; set; }

    [ObservableProperty]
    public partial int PeriodIndex { get; set; }

    [ObservableProperty]
    public partial double SegmentWidth { get; set; }

    [ObservableProperty]
    public partial double SegmentHeight { get; set; } = 40;

    [ObservableProperty]
    public partial double SegmentCornerRadius { get; set; } = 20;

    [ObservableProperty]
    public partial double SegmentStrokeThickness { get; set; } = 1;

    [ObservableProperty]
    public partial double SegmentFontSize { get; set; } = 16;

    [ObservableProperty]
    public partial double SegmentDuration { get; set; } = 250;

    [ObservableProperty]
    public partial int SelectedBackgroundIndex { get; set; }

    public ICommand ClearCommand { get; }

    public ICommand PreviousCommand { get; }

    public ICommand NextCommand { get; }

    public ChipSegmentPageViewModel()
    {
        SelectedCategory = Categories[0];
        // Fills the page width
        SegmentWidth = Math.Clamp(Math.Floor((ScreenSize.Width - 24 - 5) / Periods.Count), 60, 120);

        ClearCommand = MakeDelegateCommand(() => SelectedFruit = null);
        PreviousCommand = MakeDelegateCommand(() => PeriodIndex = Math.Max(PeriodIndex - 1, 0));
        NextCommand = MakeDelegateCommand(() => PeriodIndex = Math.Min(PeriodIndex + 1, Periods.Count - 1));
    }
}
