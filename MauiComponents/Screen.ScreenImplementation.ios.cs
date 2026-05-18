namespace MauiComponents;

using Foundation;

using UIKit;

public sealed partial class ScreenImplementation
{
#pragma warning disable CA2213
    private NSObject? screenOnObserver;
    private NSObject? screenOffObserver;

    private FullscreenWrapperViewController? fullscreenWrapper;
#pragma warning restore CA2213

    private partial void PlatformDispose()
    {
        EnableDetectScreenState(false);
        fullscreenWrapper?.Dispose();
        fullscreenWrapper = null;
    }

    // ------------------------------------------------------------
    // Fullscreen
    // ------------------------------------------------------------

    public partial void SetFullscreen(bool value)
    {
        var window = GetKeyWindow();
        if (window is null)
        {
            return;
        }

        if (value)
        {
            if (fullscreenWrapper is not null)
            {
                return;
            }

            var original = window.RootViewController!;
            fullscreenWrapper = new FullscreenWrapperViewController(original);
            window.RootViewController = fullscreenWrapper;
        }
        else
        {
            if (fullscreenWrapper is null)
            {
                return;
            }

            window.RootViewController = fullscreenWrapper.WrappedViewController;
            fullscreenWrapper = null;
        }
    }

    // ------------------------------------------------------------
    // Orientation
    // ------------------------------------------------------------

    public partial void SetOrientation(DisplayOrientation orientation) => throw new NotSupportedException();

    // ------------------------------------------------------------
    // State
    // ------------------------------------------------------------

    public partial void EnableDetectScreenState(bool value)
    {
        if (value)
        {
            if (screenOnObserver is not null)
            {
                return;
            }

            screenOnObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidBecomeActiveNotification,
                _ => RaiseScreenStateChanged(ScreenStateEventArgs.On));
            screenOffObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidEnterBackgroundNotification,
                _ => RaiseScreenStateChanged(ScreenStateEventArgs.Off));
        }
        else
        {
            if (screenOnObserver is null)
            {
                return;
            }

            NSNotificationCenter.DefaultCenter.RemoveObserver(screenOnObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(screenOffObserver!);
            screenOnObserver = null;
            screenOffObserver = null;
        }
    }

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

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private static UIWindow? GetKeyWindow() =>
        UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .SelectMany(s => s.Windows)
            .FirstOrDefault(w => w.IsKeyWindow);

    private sealed class FullscreenWrapperViewController : UIViewController
    {
        public UIViewController WrappedViewController { get; }

        public FullscreenWrapperViewController(UIViewController wrapped)
        {
            WrappedViewController = wrapped;
        }

        public override bool PrefersStatusBarHidden() => true;

        public override bool PrefersHomeIndicatorAutoHidden => true;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddChildViewController(WrappedViewController);
            View!.AddSubview(WrappedViewController.View!);
            WrappedViewController.View!.Frame = View.Bounds;
            WrappedViewController.View.AutoresizingMask =
                UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            WrappedViewController.DidMoveToParentViewController(this);
        }
    }
}
