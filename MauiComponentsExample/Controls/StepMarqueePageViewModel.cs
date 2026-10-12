namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class StepMarqueePageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial int CurrentStep { get; set; } = 2;

    [ObservableProperty]
    public partial int TotalSteps { get; set; } = 5;

    [ObservableProperty]
    public partial bool IsCountVisible { get; set; } = true;

    [ObservableProperty]
    public partial int AccentColorIndex { get; set; }

    [ObservableProperty]
    public partial int UpcomingColorIndex { get; set; }

    [ObservableProperty]
    public partial string MarqueeText { get; set; } = "Maintenance tonight from 21:00 / New items are on sale / Double points this weekend";

    [ObservableProperty]
    public partial double Speed { get; set; } = 60;

    [ObservableProperty]
    public partial double MarqueeFontSize { get; set; } = 18;

    [ObservableProperty]
    public partial int TextColorIndex { get; set; }

    public ICommand PreviousCommand { get; }

    public ICommand NextCommand { get; }

    public StepMarqueePageViewModel()
    {
        PreviousCommand = MakeDelegateCommand(() => CurrentStep = Math.Max(CurrentStep - 1, 1));
        NextCommand = MakeDelegateCommand(() => CurrentStep = Math.Min(CurrentStep + 1, TotalSteps));

        SubscribeTotalSteps(total => CurrentStep = Math.Min(CurrentStep, total));
    }
}
