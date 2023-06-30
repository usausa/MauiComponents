namespace MauiComponents;

#if ANDROID
using Android.Views;
#endif

public class DialogOptions
{
#if ANDROID
#pragma warning disable CA1819
    public Keycode[] DismissKeys { get; set; } = Array.Empty<Keycode>();
#pragma warning restore CA1819
#endif
}
