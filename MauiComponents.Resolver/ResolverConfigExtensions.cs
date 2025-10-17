namespace MauiComponents.Resolver;

using CommunityToolkit.Maui.Media;

using Smart.Resolver;

public static class ResolverConfigExtensions
{
    // Serializer

    public static ResolverConfig AddComponentsSerializer(this ResolverConfig config)
    {
        return config.AddComponentsSerializer(static _ => { });
    }

    public static ResolverConfig AddComponentsSerializer(this ResolverConfig config, Action<DefaultSerializerConfig> configure)
    {
        config.BindSingleton(_ =>
        {
            var options = new DefaultSerializerConfig();
            configure(options);
            return options;
        });
        config.BindSingleton<ISerializer, DefaultSerializer>();
        return config;
    }

    // Dialog

    public static ResolverConfig AddComponentsDialog(this ResolverConfig config)
    {
        return config.AddComponentsDialog(static _ => { });
    }

    public static ResolverConfig AddComponentsDialog(this ResolverConfig config, Action<DialogConfig> configure)
    {
        config.BindSingleton(_ =>
        {
            var options = new DialogConfig();
            configure(options);
            return options;
        });
        config.BindSingleton<IDialog, DialogImplementation>();
        config.BindSingleton(DeviceDisplay.Current);
        return config;
    }

    // Screen

    public static ResolverConfig AddComponentsScreen(this ResolverConfig config)
    {
        config.BindSingleton<IScreen, ScreenImplementation>();
        config.BindSingleton<IDisplay, DisplayImplementation>();
        config.BindSingleton(DeviceDisplay.Current);
        config.BindSingleton(Screenshot.Default);
        return config;
    }

    // Popup

    public static ResolverConfig AddComponentsPopup(this ResolverConfig config)
    {
        return config.AddComponentsPopup(static _ => { });
    }

    public static ResolverConfig AddComponentsPopup(this ResolverConfig config, Action<PopupNavigatorConfig> configure)
    {
        config.BindSingleton(_ =>
        {
            var options = new PopupNavigatorConfig();
            configure(options);
            return options;
        });
        config.BindSingleton<IPopupFactory, DefaultPopupFactory>();
        config.BindSingleton<IPopupNavigator, PopupNavigator>();
        return config;
    }

    public static ResolverConfig AddComponentsPopupPlugin<T>(this ResolverConfig config)
        where T : IPopupPlugin
    {
        config.BindSingleton(typeof(IPopupPlugin), typeof(T));
        return config;
    }

    // Location

    public static ResolverConfig AddComponentsLocation(this ResolverConfig config)
    {
        config.BindSingleton<ILocationService, LocationService>();
        config.BindSingleton(Geolocation.Default);
        return config;
    }

    // Speech

    public static ResolverConfig AddComponentsSpeech(this ResolverConfig config)
    {
        config.BindSingleton<ISpeechService, SpeechService>();
        config.BindSingleton(TextToSpeech.Default);
        config.BindSingleton(SpeechToText.Default);
        return config;
    }

    // Communication

    public static ResolverConfig AddCommunication(this ResolverConfig config)
    {
        config.BindSingleton(PhoneDialer.Default);
        config.BindSingleton(Sms.Default);
        config.BindSingleton(Email.Default);
        return config;
    }
}
