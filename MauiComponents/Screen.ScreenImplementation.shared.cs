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

    public DisplayOrientation GetOrientation() => deviceDisplay.MainDisplayInfo.Orientation;

    public partial void SetOrientation(DisplayOrientation orientation);

    public async ValueTask<Stream> TakeScreenshotAsync()
    {
        var result = await screenshot.CaptureAsync().ConfigureAwait(false);
        return await result.OpenReadAsync().ConfigureAwait(false);
    }

    public void KeepScreenOn(bool value) => deviceDisplay.KeepScreenOn = value;

    public partial void EnableDetectScreenState(bool value);

    private void RaiseScreenStateChanged(ScreenStateEventArgs args) => ScreenStateChanged?.Invoke(this, args);
}
