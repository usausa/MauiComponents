namespace MauiComponents;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponentsDialog(this IServiceCollection service)
    {
#if ANDROID
        service.AddSingleton(Dialog.Current);
#endif
        return service;
    }
}
