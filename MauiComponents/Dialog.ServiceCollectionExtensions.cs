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
        var options = new DialogOptions();
        configure(options);

        service.AddSingleton<IDialog>(new DialogImplementation(options));
#endif
        return service;
    }
}
