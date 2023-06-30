namespace MauiComponentsExample;

using System.Globalization;
using System.Windows.Input;

using MauiComponents;

using MauiComponentsExample.Dialogs;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand InformationCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand Confirm3Command { get; }
    public ICommand SelectCommand { get; }
    public ICommand LockCommand { get; }
    public ICommand LoadingCommand { get; }
    public ICommand ProgressCommand { get; }

    public ICommand PopupCommand { get; }

    public MainPageViewModel(IDialog dialog, IPopupNavigator popupNavigator)
    {
        InformationCommand = MakeAsyncCommand(async () => await dialog.InformationAsync("information"));
        ConfirmCommand = MakeAsyncCommand(async () => await dialog.ConfirmAsync("confirm"));
        Confirm3Command = MakeAsyncCommand(async () => await dialog.Confirm3Async("confirm"));
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
        PopupCommand = MakeAsyncCommand(async () =>
        {
            var result = await popupNavigator.PopupAsync<string, bool>(DialogId.Sample, DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            await dialog.InformationAsync($"Result={result}");
        });
    }
}
