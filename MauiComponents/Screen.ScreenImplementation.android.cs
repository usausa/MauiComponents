namespace MauiComponents;

using Android.App;
using Android.Content;
using Android.Content.PM;

public sealed partial class ScreenImplementation
{
#pragma warning disable CA2213
    private ScreenStateBroadcastReceiver? screenStateBroadcastReceiver;
#pragma warning restore CA2213

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    private partial void PlatformDispose()
    {
        EnableDetectScreenState(false);
    }

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial void EnableDetectScreenState(bool value)
    {
        if (value)
        {
            if (screenStateBroadcastReceiver is null)
            {
                screenStateBroadcastReceiver = new ScreenStateBroadcastReceiver(ActivityResolver.CurrentActivity, this);
                screenStateBroadcastReceiver.Register();
            }
        }
        else
        {
            if (screenStateBroadcastReceiver is not null)
            {
                screenStateBroadcastReceiver.Unregister();
                screenStateBroadcastReceiver = null;
            }
        }
    }

    private sealed class ScreenStateBroadcastReceiver : BroadcastReceiver
    {
        private readonly Activity activity;

        private readonly ScreenImplementation screen;

        public ScreenStateBroadcastReceiver(Activity activity, ScreenImplementation screen)
        {
            this.activity = activity;
            this.screen = screen;
        }

        public void Register()
        {
#pragma warning disable CA2000
            activity.RegisterReceiver(this, new IntentFilter(Intent.ActionScreenOn));
            activity.RegisterReceiver(this, new IntentFilter(Intent.ActionScreenOff));
#pragma warning restore CA2000
        }

        public void Unregister()
        {
            activity.UnregisterReceiver(this);
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent is null)
            {
                return;
            }

            if (intent.Action == Intent.ActionScreenOn)
            {
                screen.RaiseScreenStateChanged(ScreenStateEventArgs.On);
            }
            else if (intent.Action == Intent.ActionScreenOff)
            {
                screen.RaiseScreenStateChanged(ScreenStateEventArgs.Off);
            }
        }
    }
}
