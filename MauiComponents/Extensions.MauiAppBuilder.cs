namespace MauiComponents;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseCommunityToolkitServices(this MauiAppBuilder builder)
    {
        builder.Services.AddCommunityToolkitServices();
        return builder;
    }
}
