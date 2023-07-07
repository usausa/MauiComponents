namespace MauiComponents;

using Android.Content.PM;

public sealed partial class ScreenImplementation
{
    public partial void SetOrientation(DisplayOrientation orientation)
    {
        var current = GetOrientation();
        if (current == orientation)
        {
            return;
        }

        var activity = ActivityResolver.CurrentActivity;
        activity.RequestedOrientation = orientation switch
        {
            DisplayOrientation.Landscape => ScreenOrientation.Landscape,
            DisplayOrientation.Portrait => ScreenOrientation.Portrait,
            _ => activity.RequestedOrientation
        };
    }
}
