namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using Smart.Maui.ViewModels;

public sealed class ControlMenuPageViewModel : ExtendViewModelBase
{
    public ICommand NavigateCommand { get; }

    public ControlMenuPageViewModel(IServiceProvider provider)
    {
        NavigateCommand = MakeAsyncCommand<Type>(type =>
            Application.Current!.Windows[0].Page!.Navigation.PushAsync((Page)provider.GetRequiredService(type)));
    }
}
