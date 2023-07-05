namespace MauiComponents;

using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    // Serializer

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service)
    {
        service.AddSingleton<ISerializer, JsonSerializer>();
        return service;
    }

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service, Action<JsonSerializerConfig> action)
    {
        service.AddSingleton<ISerializer>(_ =>
        {
            var config = new JsonSerializerConfig();
            action(config);
            return new JsonSerializer(config);
        });
        return service;
    }

    // Dialog

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

    // Popup

    public static IServiceCollection AddComponentsPopup(this IServiceCollection service, Action<PopupNavigatorConfig> action)
    {
        service.AddSingleton(_ =>
        {
            var config = new PopupNavigatorConfig();
            action(config);
            return config;
        });
        service.AddSingleton<IPopupFactory, DefaultPopupFactory>();
        service.AddSingleton<IPopupNavigator, PopupNavigator>();
        return service;
    }

    // Location

    public static IServiceCollection AddComponentsLocation(this IServiceCollection service)
    {
        service.TryAddSingleton(Geolocation.Default);
        service.AddSingleton<ILocationManager, LocationManager>();
        return service;
    }

    // Community Toolkit

    public static IServiceCollection AddCommunityToolkitInterfaces(this IServiceCollection services)
    {
        services.AddSingleton(_ => FileSaver.Default);
        services.AddSingleton(_ => FolderPicker.Default);

        services.AddSingleton(_ => SpeechToText.Default);

        return services;
    }
}
