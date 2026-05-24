namespace MauiComponents;

using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

public sealed class DefaultPopupFactory : IPopupFactory
{
    private readonly IServiceProvider provider;

    public DefaultPopupFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public ContentView Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type) =>
        (ContentView)provider.GetRequiredService(type);
}
