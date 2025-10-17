namespace MauiComponents;

public interface IDisplay
{
#pragma warning disable CA1003
    public event Action<double> FrameUpdated;
#pragma warning restore CA1003

    void StartMonitor();

    void StopMonitor();
}
