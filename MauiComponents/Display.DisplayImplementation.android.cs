namespace MauiComponents;

using Android.Hardware.Display;
using Android.Views;

using Microsoft.Maui.Platform;

public sealed partial class DisplayImplementation : Java.Lang.Object, Choreographer.IFrameCallback
{
    private long lastFrameTimeNanosecond;

    public partial void StartMonitor()
    {
        lastFrameTimeNanosecond = 0;
        Choreographer.Instance?.PostFrameCallback(this);
    }

    public partial void StopMonitor()
    {
        Choreographer.Instance?.RemoveFrameCallback(this);
    }

    public void DoFrame(long frameTimeNanos)
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
