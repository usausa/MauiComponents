namespace MauiComponents;

using System.Text.Json;

public static partial class ServiceCollectionExtensions
{
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
