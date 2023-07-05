namespace MauiComponents;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseCommunityToolkitInterfaces(this MauiAppBuilder builder)
    {
        builder.Services.AddCommunityToolkitInterfaces();
        return builder;
    }
}
