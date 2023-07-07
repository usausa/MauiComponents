namespace MauiComponents;

public static class Screen
{
    private static ScreenImplementation? current;

    public static IScreen Current => current ??= new ScreenImplementation(DeviceDisplay.Current, Screenshot.Default);

    public static DisplayOrientation GetOrientation() =>
        Current.GetOrientation();

    public static void SetOrientation(DisplayOrientation orientation) =>
        Current.SetOrientation(orientation);

    public static ValueTask<Stream> TakeScreenshotAsync() =>
        Current.TakeScreenshotAsync();

    public static void KeepScreenOn(bool value) =>
        Current.KeepScreenOn(value);
}

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
