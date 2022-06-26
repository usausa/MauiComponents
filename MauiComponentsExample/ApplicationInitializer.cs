namespace MauiComponentsExample;

using System;

using Smart.Maui.Resolver;

public sealed class ApplicationInitializer : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        // Setup provider
        ResolveProvider.Default.Provider = services;
    }
}
