namespace MauiComponentsExample;

using System.Windows.Input;

using MauiComponents;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand InformationCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand SelectCommand { get; }
    public ICommand LockCommand { get; }
    public ICommand LoadingCommand { get; }
    public ICommand ProgressCommand { get; }

    public MainPageViewModel(IDialog dialog)
    {
        InformationCommand = MakeAsyncCommand(async () => await dialog.InformationAsync("information"));
        ConfirmCommand = MakeAsyncCommand(async () => await dialog.ConfirmAsync("confirm"));
        SelectCommand = MakeAsyncCommand(async () => await dialog.SelectAsync(new[] { "Item-1", "Item-2", "Item-3" }));
        LockCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Lock();

            await Task.Delay(3000);
        });
        LoadingCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Loading();

            loading.Update("Connecting...");
            await Task.Delay(1000);
            loading.Update("Downloading...");
            await Task.Delay(2000);
            loading.Update("Updating...");
            await Task.Delay(1000);
        });
        ProgressCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Progress();

            for (var i = 0; i <= 1000; i += 2)
            {
                loading.Update(i / 10d);
                await Task.Delay(1);
            }
        });
    }
}
