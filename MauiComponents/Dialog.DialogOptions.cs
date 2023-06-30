namespace MauiComponents;

#if ANDROID
using Android.Views;
#endif

public class DialogOptions
{
    public Color SelectColor { get; set; } = new(255, 0, 0, 64);

#if ANDROID
#pragma warning disable CA1819
    public Keycode[] DismissKeys { get; set; } = Array.Empty<Keycode>();
#pragma warning restore CA1819
#endif
}
