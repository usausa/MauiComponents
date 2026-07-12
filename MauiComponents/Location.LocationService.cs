namespace MauiComponents;

using System.Diagnostics;

using Microsoft.Maui.Devices.Sensors;

public sealed class LocationService : ILocationService, IDisposable
{
    public event EventHandler<LocationEventArgs>? LocationChanged;

    private readonly IGeolocation geolocation;

    private PeriodicTimer? timer;

    private CancellationTokenSource? cts;

    private Task? loopTask;

    public bool IsRunning => loopTask is not null;

    public LocationService(IGeolocation geolocation)
    {
        this.geolocation = geolocation;
    }

    public void Dispose()
    {
        Stop();
    }

    public void Start(GeolocationAccuracy accuracy = GeolocationAccuracy.Medium, int interval = 15000)
    {
        if (IsRunning)
        {
            return;
        }

        var period = TimeSpan.FromMilliseconds(interval);
        var source = new CancellationTokenSource();
        var periodicTimer = new PeriodicTimer(period);
        cts = source;
        timer = periodicTimer;

        loopTask = RunAsync(accuracy, period, periodicTimer, source.Token);
    }

    private async Task RunAsync(GeolocationAccuracy accuracy, TimeSpan period, PeriodicTimer periodicTimer, CancellationToken cancel)
    {
        try
        {
            do
            {
#pragma warning disable CA1031
                try
                {
                    var request = new GeolocationRequest(accuracy, period);
                    var location = await geolocation.GetLocationAsync(request, cancel).ConfigureAwait(true);
                    if (location is not null)
                    {
                        LocationChanged?.Invoke(this, new LocationEventArgs(location));
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
#pragma warning restore CA1031
            }
            while (await periodicTimer.WaitForNextTickAsync(cancel).ConfigureAwait(true));
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
        timer!.Dispose();
        cts = null;
        timer = null;
        loopTask = null;
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
            var request = new GeolocationRequest(accuracy, TimeSpan.FromMilliseconds(timeout));
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
