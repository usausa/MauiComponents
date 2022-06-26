namespace MauiComponentsExample;

using System.Reflection;

using CommunityToolkit.Maui;

using MauiComponents;

using MauiComponentsExample.Dialogs;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

#if ANDROID
        builder.Services.AddComponentsDialog();
#endif
        builder.Services.AddComponentsPopup(c =>
        {
            var ns = typeof(DialogId).Namespace!;
            c.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes
                .Where(x => x.Namespace?.StartsWith(ns, StringComparison.Ordinal) ?? false));
        });
        builder.Services.AddComponentsSerializer();

        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SampleDialog>();

        return builder.Build();
    }
}
