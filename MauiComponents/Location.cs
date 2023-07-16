namespace MauiComponents;

using Microsoft.Maui.Devices.Sensors;

public sealed class LocationEventArgs : EventArgs
{
    public Location Location { get; }

    public LocationEventArgs(Location location)
    {
        Location = location;
    }
}

public interface ILocationService
{
    event EventHandler<LocationEventArgs>? LocationChanged;

    void Start(GeolocationAccuracy accuracy, int timeout = 10000, int interval = 0);

    void Stop();

    ValueTask<Location?> GetLastLocationAsync();

    ValueTask<Location?> GetLocationAsync(GeolocationAccuracy accuracy, int timeout = 10000, CancellationToken cancel = default);
}
