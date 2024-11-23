namespace MauiComponents;

using System.Diagnostics;

using Microsoft.Maui.Devices.Sensors;

public sealed class LocationService : ILocationService, IDisposable
{
    public event EventHandler<LocationEventArgs>? LocationChanged;

    private readonly IGeolocation geolocation;

    private PeriodicTimer? timer;

    private CancellationTokenSource? cts;

    public bool IsRunning => timer is not null;

    public LocationService(IGeolocation geolocation)
    {
        this.geolocation = geolocation;
    }

    public void Dispose()
    {
        timer?.Dispose();
        cts?.Dispose();
    }

    // ReSharper disable once AsyncVoidMethod
    public async void Start(GeolocationAccuracy accuracy = GeolocationAccuracy.Medium, int interval = 15000)
    {
        if (IsRunning)
        {
            return;
        }

        var timeout = TimeSpan.FromMilliseconds(interval);
        cts = new CancellationTokenSource();
        timer = new PeriodicTimer(timeout);

        try
        {
            do
            {
                var request = new GeolocationRequest(accuracy, timeout);
                var location = await geolocation.GetLocationAsync(request, cts.Token).ConfigureAwait(true);
                if (location is not null)
                {
                    LocationChanged?.Invoke(this, new LocationEventArgs(location));
                }
            }
            while (await timer.WaitForNextTickAsync(cts.Token).ConfigureAwait(true));
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        // ReSharper disable once MethodHasAsyncOverload
        cts!.Cancel();
        cts.Dispose();
        timer = null;
        cts = null;
    }

#pragma warning disable CA1031
    public async ValueTask<Location?> GetLastLocationAsync()
    {
        try
        {
            var location = await geolocation.GetLastKnownLocationAsync().ConfigureAwait(true);
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

    public async ValueTask<Location?> GetLocationAsync(GeolocationAccuracy accuracy = GeolocationAccuracy.Medium, int timeout = 15000, CancellationToken cancel = default)
    {
#pragma warning disable CA1031
        try
        {
            var request = new GeolocationRequest(accuracy, TimeSpan.FromSeconds(timeout));
            var location = await geolocation.GetLocationAsync(request, cancel).ConfigureAwait(true);
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
}
