namespace MauiComponents;

public sealed partial class ScreenImplementation : IScreen, IDisposable
{
    public event EventHandler<ScreenStateEventArgs>? ScreenStateChanged;

    private readonly IDeviceDisplay deviceDisplay;

    private readonly IScreenshot screenshot;

    public ScreenImplementation(
        IDeviceDisplay deviceDisplay,
        IScreenshot screenshot)
    {
        this.deviceDisplay = deviceDisplay;
        this.screenshot = screenshot;
    }

    public void Dispose() => PlatformDispose();

    private partial void PlatformDispose();

    // ------------------------------------------------------------
    // Orientation
    // ------------------------------------------------------------

    public DisplayOrientation GetOrientation() => deviceDisplay.MainDisplayInfo.Orientation;

    public partial void SetOrientation(DisplayOrientation orientation);

    // ------------------------------------------------------------
    // Brightness
    // ------------------------------------------------------------

    public partial float GetScreenBrightness();

    public partial void SetScreenBrightness(float brightness);

    // ------------------------------------------------------------
    // Screenshot
    // ------------------------------------------------------------

    public async ValueTask<Stream> TakeScreenshotAsync()
    {
        var result = await screenshot.CaptureAsync().ConfigureAwait(true);
        return await result.OpenReadAsync().ConfigureAwait(true);
    }

    // ------------------------------------------------------------
    // State
    // ------------------------------------------------------------

    public void KeepScreenOn(bool value) => deviceDisplay.KeepScreenOn = value;

    public partial void EnableDetectScreenState(bool value);

#pragma warning disable IDE0051
    // ReSharper disable UnusedMember.Local
    private void RaiseScreenStateChanged(ScreenStateEventArgs args) => ScreenStateChanged?.Invoke(this, args);
    // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051
}
