namespace MauiComponents;

public sealed partial class DisplayImplementation : IDisplay
{
    public event Action<double>? FrameUpdated;

    public partial void StartMonitor();

    public partial void StopMonitor();
}
