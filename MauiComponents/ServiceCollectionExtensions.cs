namespace MauiComponents;

using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommunityToolkitInterfaces(this IServiceCollection services)
    {
        services.AddSingleton(_ => FileSaver.Default);
        services.AddSingleton(_ => FolderPicker.Default);

        services.AddSingleton(_ => SpeechToText.Default);

        return services;
    }
}
