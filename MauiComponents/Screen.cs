namespace MauiComponents;

public sealed class ScreenStateEventArgs : EventArgs
{
    internal static ScreenStateEventArgs On => new(true);

    internal static ScreenStateEventArgs Off => new(false);

    public bool ScreenOn { get; }

    internal ScreenStateEventArgs(bool screenOn)
    {
        ScreenOn = screenOn;
    }
}

public interface IScreen
{
    event EventHandler<ScreenStateEventArgs>? ScreenStateChanged;

    DisplayOrientation GetOrientation();

    void SetOrientation(DisplayOrientation orientation);

    float GetScreenBrightness();

    void SetScreenBrightness(float brightness);

    ValueTask<Stream> TakeScreenshotAsync();

    void KeepScreenOn(bool value);

    void EnableDetectScreenState(bool value);
}
