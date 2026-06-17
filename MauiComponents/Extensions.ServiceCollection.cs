namespace MauiComponents;

using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    // Dialog

    public static IServiceCollection AddComponentsDialog(this IServiceCollection service)
    {
        return service.AddComponentsDialog(static _ => { });
    }

    public static IServiceCollection AddComponentsDialog(this IServiceCollection service, Action<DialogConfig> configure)
    {
        service.TryAddSingleton(_ =>
        {
            var config = new DialogConfig();
            configure(config);
            return config;
        });
        service.TryAddSingleton<IDialog, DialogImplementation>();
        service.TryAddSingleton(DeviceDisplay.Current);
        return service;
    }

    // Screen

    public static IServiceCollection AddComponentsScreen(this IServiceCollection service)
    {
        service.TryAddSingleton<IScreen, ScreenImplementation>();
        service.TryAddSingleton<IDisplay, DisplayImplementation>();
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
        service.TryAddSingleton(_ =>
        {
            var config = new PopupNavigatorConfig();
            configure(config);
            return config;
        });
        service.TryAddSingleton<IPopupFactory, DefaultPopupFactory>();
        service.TryAddSingleton<IPopupNavigator, PopupNavigator>();
        return service;
    }

    public static IServiceCollection AddComponentsPopupPlugin<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection service)
        where T : IPopupPlugin
    {
        service.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IPopupPlugin), typeof(T)));
        return service;
    }

    // Location

    public static IServiceCollection AddComponentsLocation(this IServiceCollection service)
    {
        service.TryAddSingleton<ILocationService, LocationService>();
        service.TryAddSingleton(Geolocation.Default);
        return service;
    }

    // Speech

    public static IServiceCollection AddComponentsSpeech(this IServiceCollection service)
    {
        service.TryAddSingleton<ISpeechService, SpeechService>();
        service.TryAddSingleton(TextToSpeech.Default);
        service.TryAddSingleton(SpeechToText.Default);
        return service;
    }

    // Communication

    public static IServiceCollection AddCommunication(this IServiceCollection service)
    {
        service.TryAddSingleton(PhoneDialer.Default);
        service.TryAddSingleton(Sms.Default);
        service.TryAddSingleton(Email.Default);
        return service;
    }

    // Community Toolkit

    public static IServiceCollection AddCommunityToolkitServices(this IServiceCollection service)
    {
        service.TryAddSingleton(FileSaver.Default);
        service.TryAddSingleton(FolderPicker.Default);

        service.TryAddSingleton(SpeechToText.Default);

        return service;
    }
}
