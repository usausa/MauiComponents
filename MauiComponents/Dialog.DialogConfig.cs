namespace MauiComponents;

#if ANDROID
using Android.Views;
#endif

public class DialogConfig
{
    public Color SelectColor { get; set; } = new(255, 0, 0, 64);

    public float IndicatorSize { get; set; } = 96;

    public Color? IndicatorColor { get; set; }

    public Color LockBackgroundColor { get; set; } = new(0, 0, 0, 128);

    public Color LoadingBackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color LoadingMessageBackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color LoadingMessageColor { get; set; } = Colors.White;

    public float LoadingMessageHeight { get; set; } = 48;

    public float LoadingMessageSideMargin { get; set; } = 16;

    public float LoadingMessageCornerRadius { get; set; } = 6;

    public float LoadingMessageFontSize { get; set; } = 14;

    public Color ProgressBackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color ProgressAreaBackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color ProgressCircleColor1 { get; set; } = Colors.White;

    public Color ProgressCircleColor2 { get; set; } = Colors.Gray;

    public Color ProgressValueColor { get; set; } = Colors.White;

    public float ProgressAreaSize { get; set; } = 80;

    public float ProgressAreaCornerRadius { get; set; } = 16;

    public float ProgressSize { get; set; } = 64;

    public float ProgressWidth { get; set; } = 8;

    public float ProgressValueFontSize { get; set; } = 28;

#if ANDROID
#pragma warning disable CA1819
    public bool EnableDialogButtonFocus { get; set; }

    public bool EnablePromptEnterAction { get; set; }

    public bool EnablePromptSelectAll { get; set; }

    public Keycode[] DismissKeys { get; set; } = Array.Empty<Keycode>();

    public Keycode[] IgnorePromptDismissKeys { get; set; } = Array.Empty<Keycode>();
#pragma warning restore CA1819
#endif
}
