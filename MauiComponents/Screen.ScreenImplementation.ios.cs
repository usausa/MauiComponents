namespace MauiComponents;

using UIKit;

// TODO
public sealed partial class ScreenImplementation
{
#pragma warning disable CA1822
    private partial void PlatformDispose()
    {
    }
#pragma warning restore CA1822

    // ------------------------------------------------------------
    // Orientation
    // ------------------------------------------------------------

    public partial void SetOrientation(DisplayOrientation orientation) => throw new NotSupportedException();

    public partial void EnableDetectScreenState(bool value) => throw new NotSupportedException();

    // ------------------------------------------------------------
    // Brightness
    // ------------------------------------------------------------

#pragma warning disable CA1024
#pragma warning disable CA1822
    public partial float GetScreenBrightness()
    {
        return (float)UIScreen.MainScreen.Brightness;
    }
#pragma warning restore CA1822

#pragma warning disable CA1822
    public partial void SetScreenBrightness(float brightness)
    {
        UIScreen.MainScreen.Brightness = brightness;
    }
#pragma warning restore CA1822
#pragma warning restore CA1024
}
