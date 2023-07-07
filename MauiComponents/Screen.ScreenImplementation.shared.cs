namespace MauiComponents;

public sealed partial class ScreenImplementation : IScreen
{
    private readonly IDeviceDisplay deviceDisplay;

    private readonly IScreenshot screenshot;

    public ScreenImplementation(
        IDeviceDisplay deviceDisplay,
        IScreenshot screenshot)
    {
        this.deviceDisplay = deviceDisplay;
        this.screenshot = screenshot;
    }

    public DisplayOrientation GetOrientation() => deviceDisplay.MainDisplayInfo.Orientation;

    public partial void SetOrientation(DisplayOrientation orientation);

    public async ValueTask<Stream> TakeScreenshotAsync()
    {
        var result = await screenshot.CaptureAsync().ConfigureAwait(false);
        return await result.OpenReadAsync().ConfigureAwait(false);
    }

    public void KeepScreenOn(bool value) => deviceDisplay.KeepScreenOn = value;
}
