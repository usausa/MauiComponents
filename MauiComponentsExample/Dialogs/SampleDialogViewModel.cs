namespace MauiComponentsExample.Dialogs;

using System.Windows.Input;

using MauiComponents;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public sealed class SampleDialogViewModel : ExtendViewModelBase, IPopupInitialize<string>
{
    public NotificationValue<string> Text { get; } = new();

    public ICommand ExecuteCommand { get; }
    public ICommand CancelCommand { get; }

    public SampleDialogViewModel(
        IPopupNavigator popupNavigator)
    {
        ExecuteCommand = MakeAsyncCommand(async () => await popupNavigator.CloseAsync(true));
        CancelCommand = MakeAsyncCommand(async () => await popupNavigator.CloseAsync(false));
    }

    public void Initialize(string parameter)
    {
        Text.Value = parameter;
    }
}
