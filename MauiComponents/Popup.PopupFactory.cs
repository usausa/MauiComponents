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

    public Popup Create(Type type) => (Popup)provider.GetRequiredService(type);
}
