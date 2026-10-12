namespace MauiComponentsExample;

using System.Windows.Input;

using Smart.Maui.ViewModels;

public sealed class MainPageViewModel : ExtendViewModelBase
{
    public ICommand NavigateCommand { get; }

    public MainPageViewModel(IServiceProvider provider)
    {
        NavigateCommand = MakeAsyncCommand<Type>(type =>
            Application.Current!.Windows[0].Page!.Navigation.PushAsync((Page)provider.GetRequiredService(type)));
    }
}
