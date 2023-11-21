namespace MauiComponents;

using Android.App;
using Android.OS;

using Application = Android.App.Application;

public static class ActivityResolver
{
    public static Activity CurrentActivity { get; private set; } = default!;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
    public static void Init(Activity activity)
    {
        CurrentActivity = activity;
        activity.Application!.RegisterActivityLifecycleCallbacks(new ActivityLifecycleCallbacks());
    }

    private sealed class ActivityLifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
        {
            CurrentActivity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CurrentActivity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}
