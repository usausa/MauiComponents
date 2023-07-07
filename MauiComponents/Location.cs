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
    event EventHandler<LocationEventArgs> LocationChanged;

    int Interval { get; set; }

    void Start();

    void Stop();

    ValueTask<Location?> GetLastLocationAsync();

    ValueTask<Location?> GetLocationAsync(CancellationToken cancel = default);
}
