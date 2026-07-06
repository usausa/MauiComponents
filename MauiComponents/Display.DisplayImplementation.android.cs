namespace MauiComponents;

using Android.Views;

public sealed partial class DisplayImplementation : Java.Lang.Object, Choreographer.IFrameCallback
{
    private long lastFrameTimeNanosecond;

    private bool isMonitoring;

    public partial void StartMonitor()
    {
        if (isMonitoring)
        {
            return;
        }

        isMonitoring = true;
        lastFrameTimeNanosecond = 0;
        Choreographer.Instance?.PostFrameCallback(this);
    }

    public partial void StopMonitor()
    {
        if (!isMonitoring)
        {
            return;
        }

        isMonitoring = false;
        Choreographer.Instance?.RemoveFrameCallback(this);
    }

    void Choreographer.IFrameCallback.DoFrame(long frameTimeNanos)
    {
        if (lastFrameTimeNanosecond > 0)
        {
            var frameTimeMs = (frameTimeNanos - lastFrameTimeNanosecond) / 1_000_000.0;
            FrameUpdated?.Invoke(frameTimeMs);
        }
        lastFrameTimeNanosecond = frameTimeNanos;

        Choreographer.Instance!.PostFrameCallback(this);
    }
}
