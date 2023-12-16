namespace MauiComponentsExample;

public sealed partial class App
{
    public App(IServiceProvider provider)
    {
        InitializeComponent();

        MainPage = provider.GetRequiredService<MainPage>();
    }
}
