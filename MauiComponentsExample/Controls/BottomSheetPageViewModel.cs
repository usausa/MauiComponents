namespace MauiComponentsExample.Controls;

using System.Collections.ObjectModel;
using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class BottomSheetPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial bool IsOpen { get; set; }

    [ObservableProperty]
    public partial double HalfExpandedRatio { get; set; } = 0.5;

    [ObservableProperty]
    public partial double ExpandedRatio { get; set; } = 0.92;

    [ObservableProperty]
    public partial double CornerRadius { get; set; } = 16;

    [ObservableProperty]
    public partial double BackdropOpacity { get; set; } = 0.4;

    [ObservableProperty]
    public partial double Duration { get; set; } = 250;

    [ObservableProperty]
    public partial int SheetBackgroundColorIndex { get; set; }

    [ObservableProperty]
    public partial int GrabberColorIndex { get; set; }

    [ObservableProperty]
    public partial int ItemCount { get; set; } = 4;

    public ObservableCollection<SampleAction> Items { get; } = [];

    public ICommand OpenCommand { get; }

    public ICommand CloseCommand { get; }

    public BottomSheetPageViewModel()
    {
        OpenCommand = MakeDelegateCommand(() => IsOpen = true);
        CloseCommand = MakeDelegateCommand(() => IsOpen = false);

        UpdateItems(ItemCount);
        SubscribeItemCount(UpdateItems);
    }

    private void UpdateItems(int count)
    {
        while (Items.Count > count)
        {
            Items.RemoveAt(Items.Count - 1);
        }

        while (Items.Count < count)
        {
            Items.Add(new SampleAction($"Item {Items.Count + 1}", CloseCommand));
        }
    }
}
