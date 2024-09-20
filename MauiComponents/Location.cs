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

    GeolocationAccuracy GeolocationAccuracy { get; set; }

    int Interval { get; set; }

    public bool IsRunning { get; }

    void Start();

    void Stop();

    ValueTask<Location?> GetLastLocationAsync();

    ValueTask<Location?> GetLocationAsync(GeolocationAccuracy accuracy = GeolocationAccuracy.Medium, int timeout = 15000, CancellationToken cancel = default);
}
