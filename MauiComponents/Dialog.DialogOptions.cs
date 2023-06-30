namespace MauiComponents;

#if ANDROID
using Android.Views;
#endif

public class DialogOptions
{
    public Color SelectColor { get; set; } = new(255, 0, 0, 64);

    public Color LockBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color LoadingBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color LoadingMessageBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color LoadingMessageColor { get; set; } = Colors.White;

    public float LoadingMessageHeight { get; set; } = 64;

    public float LoadingMessageSideMargin { get; set; } = 32;

    public float LoadingMessageCornerRadius { get; set; } = 8;

    public float LoadingMessageFontSize { get; set; } = 24;

    public Color ProgressBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color ProgressAreaBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color ProgressCircleColor1 { get; set; } = Colors.White;

    public Color ProgressCircleColor2 { get; set; } = Colors.Gray;

    public Color ProgressValueColor { get; set; } = Colors.White;

    public float ProgressAreaSize { get; set; } = 80;

    public float ProgressAreaCornerRadius { get; set; } = 16;

    public float ProgressSize { get; set; } = 64;

    public float ProgressWidth { get; set; } = 8;

    public float ProgressValueFontSize { get; set; } = 24;

#if ANDROID
#pragma warning disable CA1819
    public Keycode[] DismissKeys { get; set; } = Array.Empty<Keycode>();
#pragma warning restore CA1819
#endif
}