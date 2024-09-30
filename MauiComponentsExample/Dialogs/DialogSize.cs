namespace MauiComponentsExample.Dialogs;

public static class DialogSize
{
    public static Size Large => new(
        DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.8,
        DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.8);
}
