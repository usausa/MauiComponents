namespace MauiComponents;

using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    // Serializer

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service)
    {
        return service.AddComponentsSerializer(static _ => { });
    }

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service, Action<DefaultSerializerConfig> configure)
    {
        service.AddSingleton(_ =>
        {
            var config = new DefaultSerializerConfig();
            configure(config);
            return config;
        });
        service.AddSingleton<ISerializer, DefaultSerializer>();
        return service;
    }

    // Dialog

    public static IServiceCollection AddComponentsDialog(this IServiceCollection service)
    {
        return service.AddComponentsDialog(static _ => { });
    }

    public static IServiceCollection AddComponentsDialog(this IServiceCollection service, Action<DialogConfig> configure)
    {
        service.AddSingleton(_ =>
        {
            var config = new DialogConfig();
            configure(config);
            return config;
        });
        service.AddSingleton<IDialog, DialogImplementation>();
        service.TryAddSingleton(DeviceDisplay.Current);
        return service;
    }

    // Screen

    public static IServiceCollection AddComponentsScreen(this IServiceCollection service)
    {
        service.AddSingleton<IScreen, ScreenImplementation>();
        service.TryAddSingleton(DeviceDisplay.Current);
        service.TryAddSingleton(Screenshot.Default);
        return service;
    }

    // Popup

    public static IServiceCollection AddComponentsPopup(this IServiceCollection service)
    {
        return service.AddComponentsPopup(static _ => { });
    }

    public static IServiceCollection AddComponentsPopup(this IServiceCollection service, Action<PopupNavigatorConfig> configure)
    {
        service.AddSingleton(_ =>
        {
            var config = new PopupNavigatorConfig();
            configure(config);
            return config;
        });
        service.AddSingleton<IPopupFactory, DefaultPopupFactory>();
        service.AddSingleton<IPopupNavigator, PopupNavigator>();
        return service;
    }

    public static IServiceCollection AddComponentsPopupPlugin<T>(this IServiceCollection service)
        where T : IPopupPlugin
    {
        service.AddSingleton(typeof(IPopupPlugin), typeof(T));
        return service;
    }

    // Location

    public static IServiceCollection AddComponentsLocation(this IServiceCollection service)
    {
        service.AddSingleton<ILocationService, LocationService>();
        service.TryAddSingleton(Geolocation.Default);
        return service;
    }

    // Speech

    public static IServiceCollection AddComponentsSpeech(this IServiceCollection service)
    {
        service.AddSingleton<ISpeechService, SpeechService>();
        service.TryAddSingleton(TextToSpeech.Default);
        service.TryAddSingleton(SpeechToText.Default);
        return service;
    }

    // Community Toolkit

    public static IServiceCollection AddCommunityToolkitServices(this IServiceCollection service)
    {
        service.AddSingleton(FileSaver.Default);
        service.AddSingleton(FolderPicker.Default);

        service.AddSingleton(SpeechToText.Default);

        return service;
    }
}
