namespace MauiComponents;

using CoreAnimation;

using Foundation;

public sealed partial class DisplayImplementation
{
    private CADisplayLink? displayLink;

    private double lastTimestamp;

    public partial void StartMonitor()
    {
        if (displayLink is not null)
        {
            return;
        }

        lastTimestamp = 0;
        displayLink = CADisplayLink.Create(() =>
        {
            var timestamp = displayLink!.Timestamp;
            if (lastTimestamp > 0)
            {
                var frameTimeMs = (timestamp - lastTimestamp) * 1000.0;
                FrameUpdated?.Invoke(frameTimeMs);
            }

            lastTimestamp = timestamp;
        });
        displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Default);
    }

    public partial void StopMonitor()
    {
        displayLink?.Invalidate();
        displayLink = null;
        lastTimestamp = 0;
    }
}
