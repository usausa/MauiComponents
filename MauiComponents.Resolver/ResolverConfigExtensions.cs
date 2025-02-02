namespace MauiComponents.Resolver;

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
        return config;
    }

    // Screen

    public static ResolverConfig AddComponentsScreen(this ResolverConfig config)
    {
        config.BindSingleton(DeviceDisplay.Current);
        config.BindSingleton(Screenshot.Default);
        config.BindSingleton<IScreen, ScreenImplementation>();
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
        config.BindSingleton(Geolocation.Default);
        config.BindSingleton<ILocationService, LocationService>();
        return config;
    }
}
