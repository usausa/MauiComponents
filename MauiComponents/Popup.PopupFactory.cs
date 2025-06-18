namespace MauiComponents;

using CommunityToolkit.Maui.Views;

using Microsoft.Extensions.DependencyInjection;

public sealed class DefaultPopupFactory : IPopupFactory
{
    private readonly IServiceProvider provider;

    public DefaultPopupFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public ContentView Create(Type type) => (ContentView)provider.GetRequiredService(type);
}
