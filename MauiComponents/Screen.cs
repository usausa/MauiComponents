namespace MauiComponents;

public interface IScreen
{
    DisplayOrientation GetOrientation();

    void SetOrientation(DisplayOrientation orientation);

    ValueTask<Stream> TakeScreenshotAsync();

    void KeepScreenOn(bool value);
}
