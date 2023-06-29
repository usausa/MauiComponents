namespace MauiComponents;

using System.Diagnostics;

using Microsoft.Maui.Devices.Sensors;

public sealed class LocationManager : ILocationManager, IDisposable
{
    public event EventHandler<LocationEventArgs>? LocationChanged;

    private readonly IGeolocation geolocation;

    private bool running;

    private CancellationTokenSource? cts;

    public int Timeout { get; set; } = 10000;

    public GeolocationAccuracy Accuracy { get; set; } = GeolocationAccuracy.Medium;

    public int Interval { get; set; } = 5000;

    public LocationManager()
        : this(Geolocation.Default)
    {
    }

    public LocationManager(IGeolocation geolocation)
    {
        this.geolocation = geolocation;
    }

    public void Dispose()
    {
        cts?.Dispose();
    }

    public void Start()
    {
        if (running)
        {
            return;
        }

        running = true;

        cts = new CancellationTokenSource();
#pragma warning disable CA2012
        _ = LocationLoop(cts.Token);
#pragma warning restore CA2012
    }

    public void Stop()
    {
        if (!running)
        {
            return;
        }

        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        running = false;
    }

#pragma warning disable CA1031
    public async ValueTask<Location?> GetLastLocationAsync()
    {
        try
        {
            var location = await geolocation.GetLastKnownLocationAsync().ConfigureAwait(false);
            if (location != null)
            {
                return location;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
#pragma warning restore CA1031

        return null;
    }

    public async ValueTask<Location?> GetLocationAsync(CancellationToken cancel = default)
    {
#pragma warning disable CA1031
        try
        {
            var request = new GeolocationRequest(Accuracy, TimeSpan.FromSeconds(Timeout));
            var location = await geolocation.GetLocationAsync(request, cancel).ConfigureAwait(false);
            if (location != null)
            {
                return location;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
#pragma warning restore CA1031

        return null;
    }

    private async ValueTask LocationLoop(CancellationToken cancel)
    {
#pragma warning disable CA1031
        try
        {
            while (!cancel.IsCancellationRequested)
            {
                var request = new GeolocationRequest(Accuracy, TimeSpan.FromSeconds(Timeout));
                var location = await geolocation.GetLocationAsync(request).ConfigureAwait(false);
                if (location is not null)
                {
                    LocationChanged?.Invoke(this, new LocationEventArgs(location));
                }

                await Task.Delay(Interval, cancel).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
#pragma warning restore CA1031
    }
}
