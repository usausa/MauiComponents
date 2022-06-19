namespace MauiComponents;

using System.Text.Json;

public static class ServiceCollectionExtensions
{
#if ANDROID
    public static IServiceCollection AddComponentsDialog(this IServiceCollection service)
    {
        service.AddSingleton<IDialog>(Dialog.Current);
        return service;
    }
#endif

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service)
    {
        service.AddSingleton<ISerializer, JsonSerializer>();
        return service;
    }

    public static IServiceCollection AddComponentsSerializer(this IServiceCollection service, Action<JsonSerializerOptions> action)
    {
        var option = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        action(option);
        service.AddSingleton<ISerializer>(new JsonSerializer(option));
        return service;
    }
}
