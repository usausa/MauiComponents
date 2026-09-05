namespace MauiComponentsExample;

#pragma warning disable CA1724
public sealed partial class App
{
    private readonly IServiceProvider provider;

    public App(IServiceProvider provider)
    {
        this.provider = provider;

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(provider.GetRequiredService<MainPage>());
    }
}
#pragma warning restore CA1724
