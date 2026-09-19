namespace MauiComponents;

using Foundation;

using UIKit;

#pragma warning disable CA1822
public sealed partial class WiFiManager
{
    public partial bool IsSupported => false;

    public partial bool IsRadioOn => false;

    public partial bool StartScan() => false;

    public partial void OpenSettings()
    {
        using var url = new NSUrl(UIApplication.OpenSettingsUrlString);
        UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions(), null);
    }

    private partial void Start()
    {
    }

    private partial void Stop()
    {
        UpdateAccessPoints([]);
        Update(null);
    }
}
#pragma warning restore CA1822
