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
        configure(((DialogImplementation)Dialog.Current).Options);

        service.AddSingleton(Dialog.Current);
#endif
        return service;
    }
}
