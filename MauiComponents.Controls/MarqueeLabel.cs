namespace MauiComponents.Controls;

public sealed class MarqueeLabel : ContentView
{
    private const string AnimationName = "MarqueeScroll";

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MarqueeLabel),
        string.Empty,
        propertyChanged: static (bindable, _, _) => ((MarqueeLabel)bindable).Restart());

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(MarqueeLabel),
        Colors.Black);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(MarqueeLabel),
        14d,
        propertyChanged: static (bindable, _, _) => ((MarqueeLabel)bindable).Restart());

    public static readonly BindableProperty SpeedProperty = BindableProperty.Create(
        nameof(Speed),
        typeof(double),
        typeof(MarqueeLabel),
        60d,
        propertyChanged: static (bindable, _, _) => ((MarqueeLabel)bindable).Restart());

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public double Speed
    {
        get => (double)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    private readonly Label label;

    private bool running;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public MarqueeLabel()
    {
        label = new Label
        {
            LineBreakMode = LineBreakMode.NoWrap,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };
        label.SetBinding(Label.TextProperty, static (MarqueeLabel v) => v.Text, source: this);
        label.SetBinding(Label.TextColorProperty, static (MarqueeLabel v) => v.TextColor, source: this);
        label.SetBinding(Label.FontSizeProperty, static (MarqueeLabel v) => v.FontSize, source: this);

        Content = new Grid
        {
            IsClippedToBounds = true,
            Children = { label }
        };

        Loaded += (_, _) => Restart();
        Unloaded += (_, _) => Stop();
        SizeChanged += (_, _) => Restart();
    }

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private void Restart()
    {
        Stop();

        if (!IsLoaded || (Width <= 0d) || String.IsNullOrEmpty(Text) || (Speed <= 0d))
        {
            return;
        }

        var textWidth = label.Measure(Double.PositiveInfinity, Double.PositiveInfinity).Width;
        var start = Width;
        var end = -textWidth;
        var length = (uint)Math.Max(1d, (start - end) / Speed * 1000d);

        running = true;
        this.Animate(
            AnimationName,
            v => label.TranslationX = v,
            start,
            end,
            16,
            length,
            Easing.Linear,
            null,
            () => running);
    }

    private void Stop()
    {
        running = false;
        this.AbortAnimation(AnimationName);
    }
}
