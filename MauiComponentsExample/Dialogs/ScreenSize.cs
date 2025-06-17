namespace MauiComponentsExample.Dialogs;

public static class ScreenSize
{
    public static double Height { get; } = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;

    public static double Width { get; } = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;

    // Dialog

    public static double LargeDialogWidth => Width * 0.8;
}
