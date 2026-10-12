namespace MauiComponentsExample;

#if ANDROID
using Android.Views;
#endif

using BunnyTail.DependencyInjection;

using CommunityToolkit.Maui;

using MauiComponents;

using MauiComponentsExample.Dialogs;

#if DEBUG
using Microsoft.Extensions.Logging;
#endif

using Smart.Maui;

public static partial class MauiProgram
{
    private const string DialogsNamespace = "MauiComponentsExample.Dialogs";

    private const string ControlsNamespace = "MauiComponentsExample.Controls";

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
                    c.IndicatorColor = new Color(27, 110, 194);
                    c.LoadingMessageBackgroundColor = Colors.White;
                    c.LoadingMessageColor = Colors.Black;
                    c.ProgressValueColor = Colors.Black;
                    c.ProgressAreaBackgroundColor = Colors.White;
                    c.ProgressCircleColor1 = new Color(27, 110, 194);
                    c.ProgressCircleColor2 = new Color(224, 224, 224);

                    c.DismissKeys = [Keycode.Escape, Keycode.Del];
                    c.IgnorePromptDismissKeys = [Keycode.Del];
                    c.EnableDialogButtonFocus = true;
                    c.EnablePromptEnterAction = true;
                    c.EnablePromptSelectAll = true;
                });
#endif
                services.AddComponentsPopup(c =>
                {
                    c.AutoRegister(ViewRegistry.DialogSource());
                });
            })
            .ConfigureContainer(new GeneratedServiceProviderFactory(static options => options.TrackTransientDisposables = false), ConfigureContainer);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureContainer(IServiceCollection services)
    {
        services.AddSingleton<IMauiInitializeService, ApplicationInitializer>();

        services.AddTransient<MainPage>();
        services.AddTransient<MainPageViewModel>();
        services.AddDialogs();
        services.AddControls();
    }

    // ReSharper disable UnusedMethodReturnValue.Local
    [ComponentRegistration(Lifetime.Transient, "Dialog$|Page$|ViewModel$", Namespace = DialogsNamespace)]
    private static partial IServiceCollection AddDialogs(this IServiceCollection services);

    [ComponentRegistration(Lifetime.Transient, "Page$|ViewModel$", Namespace = ControlsNamespace)]
    private static partial IServiceCollection AddControls(this IServiceCollection services);
    // ReSharper restore UnusedMethodReturnValue.Local
}
