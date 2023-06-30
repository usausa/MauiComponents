namespace MauiComponents;

public static class Dialog
{
    private static DialogImplementation? current;

    public static IDialog Current => current ??= new DialogImplementation();

    public static ValueTask InformationAsync(string message, string? title = null, string ok = "OK") =>
        Current.InformationAsync(message, title, ok);

    public static ValueTask<bool> ConfirmAsync(string message, bool defaultPositive = false, string? title = null, string ok = "OK", string cancel = "Cancel") =>
        Current.ConfirmAsync(message, defaultPositive, title, ok, cancel);

    public static ValueTask<Confirm3Result> Confirm3Async(string message, bool defaultPositive = false, string? title = null, string ok = "OK", string cancel = "Cancel", string neutral = "Maybe") =>
        Current.Confirm3Async(message, defaultPositive, title, ok, cancel, neutral);

    public static ValueTask<int> SelectAsync(string[] items, int selected = -1, string? title = null) =>
        Current.SelectAsync(items, selected, title);

    public static IDisposable Lock() =>
        Current.Lock();

    public static IDisposable Indicator() =>
        Current.Indicator();

    public static ILoading Loading(string text = "") =>
        Current.Loading(text);

    public static IProgress Progress() =>
        Current.Progress();

    public static void Snackbar(string message, int duration = 1000, Color? color = null, Color? textColor = null) =>
        Current.Snackbar(message, duration, color, textColor);
}

internal sealed partial class DialogImplementation : IDialog
{
    public DialogOptions Options { get; } = new();

    public partial ValueTask InformationAsync(string message, string? title, string ok);

    public partial ValueTask<bool> ConfirmAsync(string message, bool defaultPositive, string? title, string ok, string cancel);

    public partial ValueTask<Confirm3Result> Confirm3Async(string message, bool defaultPositive, string? title, string ok, string cancel, string neutral);

    public partial ValueTask<int> SelectAsync(string[] items, int selected, string? title);

    public partial IDisposable Indicator();

    public partial void Snackbar(string message, int duration, Color? color, Color? textColor);

    public IDisposable Lock()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LockOverlay(window, Options);
    }

    public ILoading Loading(string text = "")
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LoadingOverlay(window, Options, text);
    }

    public IProgress Progress()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new ProgressOverlay(window, Options);
    }

    private sealed class LockOverlay : WindowOverlay, IDisposable
    {
        public LockOverlay(IWindow window, DialogOptions options)
            : base(window)
        {
            AddWindowElement(new ElementOverlay(options));
            EnableDrawableTouchHandling = true;

            window.AddOverlay(this);
        }

        public void Dispose()
        {
            Window.RemoveOverlay(this);
        }

        private sealed class ElementOverlay : IWindowOverlayElement
        {
            private readonly DialogOptions options;

            public ElementOverlay(DialogOptions options)
            {
                this.options = options;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = options.LockBackgroundColor;
                canvas.FillRectangle(dirtyRect);
            }

            public bool Contains(Point point) => true;
        }
    }

    private sealed class LoadingOverlay : WindowOverlay, ILoading
    {
        private readonly ElementOverlay overlay;

        public LoadingOverlay(IWindow window, DialogOptions options, string text)
            : base(window)
        {
            overlay = new ElementOverlay(options) { Text = text };
            AddWindowElement(overlay);
            EnableDrawableTouchHandling = true;

            window.AddOverlay(this);
        }

        public void Dispose()
        {
            Window.RemoveOverlay(this);
        }

        public void Update(string text)
        {
            overlay.Text = text;
            Invalidate();
        }

        private sealed class ElementOverlay : IWindowOverlayElement
        {
            private readonly DialogOptions options;

            public string Text { get; set; } = string.Empty;

            public ElementOverlay(DialogOptions options)
            {
                this.options = options;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = options.LoadingBackgroundColor;
                canvas.FillRectangle(dirtyRect);

                if (!String.IsNullOrEmpty(Text))
                {
                    var messageRect = new RectF(
                        options.LoadingMessageSideMargin,
                        (dirtyRect.Height / 2) - (options.LoadingMessageHeight / 2),
                        dirtyRect.Width - (options.LoadingMessageSideMargin * 2),
                        options.LoadingMessageHeight);

                    canvas.FillColor = options.LoadingMessageBackgroundColor;
                    canvas.FillRoundedRectangle(messageRect, options.LoadingMessageCornerRadius);

                    canvas.FontColor = options.LoadingMessageColor;
                    canvas.FontSize = options.LoadingMessageFontSize;
                    canvas.DrawString(Text, messageRect, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
            }

            public bool Contains(Point point) => true;
        }
    }

    private sealed class ProgressOverlay : WindowOverlay, IProgress
    {
        private readonly ElementOverlay overlay;

        public ProgressOverlay(IWindow window, DialogOptions options)
            : base(window)
        {
            overlay = new ElementOverlay(options);
            AddWindowElement(overlay);
            EnableDrawableTouchHandling = true;

            window.AddOverlay(this);
        }

        public void Dispose()
        {
            Window.RemoveOverlay(this);
        }

        public void Update(double value)
        {
            overlay.Value = value switch
            {
                > 100 => 100,
                < 0 => 0,
                _ => value
            };
            Invalidate();
        }

        private sealed class ElementOverlay : IWindowOverlayElement
        {
            private readonly DialogOptions options;

            public double Value { get; set; }

            public ElementOverlay(DialogOptions options)
            {
                this.options = options;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = options.ProgressBackgroundColor;
                canvas.FillRectangle(dirtyRect);

                canvas.FillColor = options.ProgressAreaBackgroundColor;
                canvas.FillRoundedRectangle(
                    new RectF(
                        (dirtyRect.Width / 2) - options.ProgressAreaSize,
                        (dirtyRect.Height / 2) - options.ProgressAreaSize,
                        options.ProgressAreaSize * 2,
                        options.ProgressAreaSize * 2),
                    options.ProgressAreaCornerRadius);

                var arcRect = new RectF(
                    (dirtyRect.Width / 2) - options.ProgressSize,
                    (dirtyRect.Height / 2) - options.ProgressSize,
                    options.ProgressSize * 2,
                    options.ProgressSize * 2);

                canvas.StrokeSize = options.ProgressWidth;
                canvas.StrokeColor = options.ProgressCircleColor2;
                canvas.DrawArc(arcRect, 0, 360, false, false);

                canvas.StrokeColor = options.ProgressCircleColor1;
                canvas.DrawArc(arcRect, 90, 90 - (int)(360 * Value / 100), true, false);

                canvas.FontColor = options.ProgressValueColor;
                canvas.FontSize = options.ProgressValueFontSize;
                canvas.DrawString($"{Value:F1}%", arcRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            }

            public bool Contains(Point point) => true;
        }
    }
}
