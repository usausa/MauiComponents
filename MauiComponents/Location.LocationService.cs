namespace MauiComponents;

using System.Diagnostics;

using Microsoft.Maui.Devices.Sensors;

public sealed class LocationService : ILocationService, IDisposable
{
    public event EventHandler<LocationEventArgs>? LocationChanged;

    private readonly IGeolocation geolocation;

    private bool running;

    private CancellationTokenSource? cts;

    public LocationService(IGeolocation geolocation)
    {
        this.geolocation = geolocation;
    }

    public void Dispose()
    {
        cts?.Dispose();
    }

    public void Start(GeolocationAccuracy accuracy, int timeout = 10000, int interval = 0)
    {
        if (running)
        {
            return;
        }

        running = true;

        cts = new CancellationTokenSource();
#pragma warning disable CA2012
        _ = LocationLoop(accuracy, timeout, interval, cts.Token);
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

    public async ValueTask<Location?> GetLocationAsync(GeolocationAccuracy accuracy, int timeout = 10000, CancellationToken cancel = default)
    {
#pragma warning disable CA1031
        try
        {
            var request = new GeolocationRequest(accuracy, TimeSpan.FromSeconds(timeout));
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

    private async ValueTask LocationLoop(GeolocationAccuracy accuracy, int timeout, int interval, CancellationToken cancel)
    {
#pragma warning disable CA1031
        try
        {
            while (!cancel.IsCancellationRequested)
            {
                var request = new GeolocationRequest(accuracy, TimeSpan.FromSeconds(timeout));
                var location = await geolocation.GetLocationAsync(request).ConfigureAwait(false);
                if (location is not null)
                {
                    LocationChanged?.Invoke(this, new LocationEventArgs(location));
                }

                await Task.Delay(interval, cancel).ConfigureAwait(false);
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
