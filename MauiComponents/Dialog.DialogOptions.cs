namespace MauiComponents;

#if ANDROID
using Android.Views;
#endif

public class DialogOptions
{
    public Color SelectColor { get; set; } = Colors.Red;

    public int SelectColorAlpha { get; set; } = 64;

#if ANDROID
#pragma warning disable CA1819
    public Keycode[] DismissKeys { get; set; } = Array.Empty<Keycode>();
#pragma warning restore CA1819
#endif
}
