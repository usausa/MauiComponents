namespace MauiComponents.Resolver;

using Smart.Resolver;

public static class ResolverConfigExtensions
{
    // Serializer

    public static ResolverConfig UseComponentsSerializer(this ResolverConfig config)
    {
        return config.UseComponentsSerializer(_ => { });
    }

    public static ResolverConfig UseComponentsSerializer(this ResolverConfig config, Action<DefaultSerializerConfig> configure)
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

#if ANDROID
    public static ResolverConfig UseComponentsDialog(this ResolverConfig config)
    {
        return config.UseComponentsDialog(_ => { });
    }

    public static ResolverConfig UseComponentsDialog(this ResolverConfig config, Action<DialogConfig> configure)
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
#endif

    // Popup

    public static ResolverConfig UseComponentsPopup(this ResolverConfig config)
    {
        return config.UseComponentsPopup(_ => { });
    }

    public static ResolverConfig UseComponentsPopup(this ResolverConfig config, Action<PopupNavigatorConfig> configure)
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

    public static ResolverConfig UseComponentsLocation(this ResolverConfig config)
    {
        config.BindSingleton<ILocationManager, LocationManager>();
        return config;
    }
}
