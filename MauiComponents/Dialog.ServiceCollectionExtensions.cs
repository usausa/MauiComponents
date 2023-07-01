namespace MauiComponents;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponentsDialog(this IServiceCollection service)
    {
        return service.AddComponentsDialog(_ => { });
    }

    public static IServiceCollection AddComponentsDialog(this IServiceCollection service, Action<DialogOptions> configure)
    {
#if ANDROID

        service.AddSingleton<DialogOptions>(_ =>
        {
            var options = new DialogOptions();
            configure(options);
            return options;
        });
        service.AddSingleton<IDialog, DialogImplementation>();
#endif
        return service;
    }
}
