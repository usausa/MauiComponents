namespace MauiComponents.Resolver;

using Smart.Resolver;

public static class ResolverConfigExtensions
{
    // Serializer

    public static ResolverConfig AddComponentsSerializer(this ResolverConfig config)
    {
        return config.AddComponentsSerializer(_ => { });
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
        return config.AddComponentsDialog(_ => { });
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
        config.BindSingleton<IScreen, ScreenImplementation>();
        return config;
    }

    // Popup

    public static ResolverConfig AddComponentsPopup(this ResolverConfig config)
    {
        return config.AddComponentsPopup(_ => { });
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

    // Location

    public static ResolverConfig AddComponentsLocation(this ResolverConfig config)
    {
        config.BindSingleton<ILocationManager, LocationManager>();
        return config;
    }
}
