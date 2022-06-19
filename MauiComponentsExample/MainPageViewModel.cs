namespace MauiComponentsExample;

using System.Windows.Input;

using MauiComponents;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand InformationCommand { get; }

    public ICommand ConfirmCommand { get; }

    public ICommand SelectCommand { get; }

    public MainPageViewModel(IDialog dialog)
    {
        InformationCommand = MakeAsyncCommand(async () => await dialog.Information("information"));
        ConfirmCommand = MakeAsyncCommand(async () => await dialog.Confirm("confirm"));
        SelectCommand = MakeAsyncCommand(async () => await dialog.Select(new[] { "Item-1", "Item-2", "Item-3" }));
    }
}
