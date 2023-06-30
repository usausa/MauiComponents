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

    public static ILoading Loading(string text = "") =>
        Current.Loading(text);

    public static IProgress Progress() =>
        Current.Progress();
}

internal sealed partial class DialogImplementation : IDialog
{
    public DialogOptions Options { get; } = new();

    public partial ValueTask InformationAsync(string message, string? title, string ok);

    public partial ValueTask<bool> ConfirmAsync(string message, bool defaultPositive, string? title, string ok, string cancel);

    public partial ValueTask<Confirm3Result> Confirm3Async(string message, bool defaultPositive, string? title, string ok, string cancel, string neutral);

    public partial ValueTask<int> SelectAsync(string[] items, int selected, string? title);

    public IDisposable Lock()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LockOverlay(window);
    }

    public ILoading Loading(string text = "")
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LoadingOverlay(window, text);
    }

    public IProgress Progress()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new ProgressOverlay(window);
    }

    private sealed class LockOverlay : WindowOverlay, IDisposable
    {
        public LockOverlay(IWindow window)
            : base(window)
        {
            AddWindowElement(new ElementOverlay());
            EnableDrawableTouchHandling = true;

            window.AddOverlay(this);
        }

        public void Dispose()
        {
            Window.RemoveOverlay(this);
        }

        private sealed class ElementOverlay : IWindowOverlayElement
        {
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
                canvas.FillRectangle(dirtyRect);
            }

            public bool Contains(Point point) => true;
        }
    }

    private sealed class LoadingOverlay : WindowOverlay, ILoading
    {
        private readonly ElementOverlay overlay;

        public LoadingOverlay(IWindow window, string text)
            : base(window)
        {
            overlay = new ElementOverlay { Text = text };
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
            public string Text { get; set; } = string.Empty;

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
                canvas.FillRectangle(dirtyRect);

                if (!String.IsNullOrEmpty(Text))
                {
                    var messageRect = new RectF(32, (dirtyRect.Height / 2) - 32, dirtyRect.Width - 64, 64);

                    canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
                    canvas.FillRoundedRectangle(messageRect, 8);

                    canvas.FontColor = Colors.White;
                    canvas.FontSize = 24;
                    canvas.DrawString(Text, messageRect, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
            }

            public bool Contains(Point point) => true;
        }
    }

    private sealed class ProgressOverlay : WindowOverlay, IProgress
    {
        private readonly ElementOverlay overlay;

        public ProgressOverlay(IWindow window)
            : base(window)
        {
            overlay = new ElementOverlay();
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
            public double Value { get; set; }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
                canvas.FillRectangle(dirtyRect);

                canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
                canvas.FillRoundedRectangle(new RectF((dirtyRect.Width / 2) - 80, (dirtyRect.Height / 2) - 80, 160, 160), 16);

                var arcRect = new RectF((dirtyRect.Width / 2) - 64, (dirtyRect.Height / 2) - 64, 128, 128);

                canvas.StrokeSize = 8;
                canvas.StrokeColor = Colors.Gray;
                canvas.DrawArc(arcRect, 0, 360, false, false);

                canvas.StrokeColor = Colors.White;
                canvas.DrawArc(arcRect, 90, 90 - (int)(360 * Value / 100), true, false);

                canvas.FontColor = Colors.White;
                canvas.FontSize = 24;
                canvas.DrawString($"{Value:F1}%", arcRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            }

            public bool Contains(Point point) => true;
        }
    }
}
