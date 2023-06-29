namespace MauiComponents;

using Microsoft.Extensions.DependencyInjection.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponentsLocation(this IServiceCollection service)
    {
        service.TryAddSingleton(Geolocation.Default);
        service.AddSingleton<ILocationManager, LocationManager>();
        return service;
    }
}
