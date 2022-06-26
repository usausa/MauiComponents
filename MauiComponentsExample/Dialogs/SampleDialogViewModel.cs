namespace MauiComponentsExample.Dialogs;

using System.Windows.Input;

using MauiComponents;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public class SampleDialogViewModel : ViewModelBase, IPopupInitialize<string>
{
    public PopupController<bool> Popup { get; } = new();

    public NotificationValue<string> Text { get; } = new();

    public ICommand ExecuteCommand { get; }
    public ICommand CancelCommand { get; }

    public SampleDialogViewModel()
    {
        ExecuteCommand = MakeDelegateCommand(() => Popup.Close(true));
        CancelCommand = MakeDelegateCommand(() => Popup.Close());
    }

    public void Initialize(string parameter)
    {
        Text.Value = parameter;
    }
}
