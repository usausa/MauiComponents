namespace MauiComponents;

#if ANDROID
using Microsoft.Maui.LifecycleEvents;
#endif

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMauiComponents(this MauiAppBuilder builder)
    {
#if ANDROID
        builder
            .ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android =>
                {
                    android.OnCreate((activity, _) =>
                    {
                        ActivityResolver.Init(activity);
                    });
                });
            });
#endif

        return builder;
    }

    public static MauiAppBuilder UseCommunityToolkitServices(this MauiAppBuilder builder)
    {
        builder.Services.AddCommunityToolkitServices();
        return builder;
    }
}
