namespace MauiComponentsExample;

using System.Reflection;

#if ANDROID
using Android.Views;
#endif

using CommunityToolkit.Maui;

using MauiComponents;

using MauiComponentsExample.Dialogs;

using Smart.Maui;
using Smart.Resolver;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureService(services =>
            {
#if ANDROID
                services.AddComponentsDialog(c =>
                {
                    c.DismissKeys = new[] { Keycode.Escape };
                    c.LoadingMessageBackgroundColor = Colors.White;
                    c.LoadingMessageColor = Colors.Black;
                    c.ProgressValueColor = Colors.Black;
                    c.ProgressAreaBackgroundColor = Colors.White;
                    c.ProgressCircleColor1 = new Color(27, 110, 194);
                    c.ProgressCircleColor2 = new Color(224, 224, 224);
                });
#endif
                services.AddComponentsPopup(c =>
                {
                    var ns = typeof(DialogId).Namespace!;
                    c.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes
                        .Where(x => x.Namespace?.StartsWith(ns, StringComparison.Ordinal) ?? false));
                });
                services.AddComponentsSerializer();
            })
            .ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

        return builder.Build();
    }

    private static void ConfigureContainer(ResolverConfig config)
    {
        config
            .UseAutoBinding()
            .UseArrayBinding()
            .UseAssignableBinding()
            .UsePropertyInjector();

        config.BindSingleton<IMauiInitializeService, ApplicationInitializer>();
    }
}
