namespace MauiComponents.Controls;

using Microsoft.Maui.Controls.Shapes;

public sealed class BottomSheetView : Grid
{
    private const string AnimationName = "SheetMove";

    private const uint Duration = 250;

    private const uint AnimationRate = 16;

    private const uint MinFlingDuration = 80;

    private const uint MaxFlingDuration = 400;

    private const double FlingSlope = Math.PI / 2;

    private const double SwipeDistance = 16;

    private const double SwipeVelocity = 0.2;

    private const double DirectionDistance = 1;

    private const long PauseInterval = 100;

    private const double GrabberWidth = 40;

    private const double GrabberHeight = 4;

    private const double GrabberTopMargin = 10;

    private const double GrabberBottomMargin = 6;

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private readonly BoxView backdrop;

    private readonly BoxView grabber;

    private readonly Border sheet;

    private readonly Grid body;

    private readonly ContentView contentHost;

    private double panStart;

    private double panTotal;

    private double panVelocity;

    private int panDirection;

    private long panTimestamp;

    private bool expanded;

    private bool panning;

    private uint? closeDuration;

    private bool pendingOpen;

    private bool attached;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(BottomSheetView),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).OnIsOpenChanged((bool)newValue));

    public static readonly BindableProperty SheetContentProperty = BindableProperty.Create(
        nameof(SheetContent),
        typeof(View),
        typeof(BottomSheetView),
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).contentHost.Content = (View?)newValue);

    public static readonly BindableProperty SheetTemplateProperty = BindableProperty.Create(
        nameof(SheetTemplate),
        typeof(DataTemplate),
        typeof(BottomSheetView));

    public static readonly BindableProperty HalfExpandedRatioProperty = BindableProperty.Create(
        nameof(HalfExpandedRatio),
        typeof(double),
        typeof(BottomSheetView),
        0.5d);

    public static readonly BindableProperty ExpandedRatioProperty = BindableProperty.Create(
        nameof(ExpandedRatio),
        typeof(double),
        typeof(BottomSheetView),
        0.92d,
        propertyChanged: static (bindable, _, _) => ((BottomSheetView)bindable).UpdateSheetSize());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(BottomSheetView),
        16d,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).sheet.StrokeShape = CreateShape((double)newValue));

    public static readonly BindableProperty SheetBackgroundColorProperty = BindableProperty.Create(
        nameof(SheetBackgroundColor),
        typeof(Color),
        typeof(BottomSheetView),
        Colors.White,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).sheet.BackgroundColor = (Color)newValue);

    public static readonly BindableProperty GrabberColorProperty = BindableProperty.Create(
        nameof(GrabberColor),
        typeof(Color),
        typeof(BottomSheetView),
        Color.FromArgb("#BDBDBD"),
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).grabber.Color = (Color)newValue);

    public static readonly BindableProperty BackdropOpacityProperty = BindableProperty.Create(
        nameof(BackdropOpacity),
        typeof(double),
        typeof(BottomSheetView),
        0.4d);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public View? SheetContent
    {
        get => (View?)GetValue(SheetContentProperty);
        set => SetValue(SheetContentProperty, value);
    }

    public DataTemplate? SheetTemplate
    {
        get => (DataTemplate?)GetValue(SheetTemplateProperty);
        set => SetValue(SheetTemplateProperty, value);
    }

    public double HalfExpandedRatio
    {
        get => (double)GetValue(HalfExpandedRatioProperty);
        set => SetValue(HalfExpandedRatioProperty, value);
    }

    public double ExpandedRatio
    {
        get => (double)GetValue(ExpandedRatioProperty);
        set => SetValue(ExpandedRatioProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Color SheetBackgroundColor
    {
        get => (Color)GetValue(SheetBackgroundColorProperty);
        set => SetValue(SheetBackgroundColorProperty, value);
    }

    public Color GrabberColor
    {
        get => (Color)GetValue(GrabberColorProperty);
        set => SetValue(GrabberColorProperty, value);
    }

    public double BackdropOpacity
    {
        get => (double)GetValue(BackdropOpacityProperty);
        set => SetValue(BackdropOpacityProperty, value);
    }

    private double SheetHeight { get; set; }

    private double HalfY => Math.Max(0, SheetHeight - (Height * HalfExpandedRatio));

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public BottomSheetView()
    {
        IsVisible = false;

        backdrop = new BoxView { Color = Colors.Black, BackgroundColor = Colors.Transparent, Opacity = 0 };
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => IsOpen = false;
        backdrop.GestureRecognizers.Add(tap);

        grabber = new BoxView
        {
            WidthRequest = GrabberWidth,
            HeightRequest = GrabberHeight,
            CornerRadius = GrabberHeight / 2,
            Color = GrabberColor,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, GrabberTopMargin, 0, GrabberBottomMargin)
        };
        contentHost = new ContentView();
        body = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            BackgroundColor = Colors.Transparent
        };
        body.Add(grabber);
        body.Add(contentHost, 0, 1);
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        body.GestureRecognizers.Add(pan);

        sheet = new Border
        {
            VerticalOptions = LayoutOptions.End,
            BackgroundColor = SheetBackgroundColor,
            StrokeThickness = 0,
            StrokeShape = CreateShape(CornerRadius),
            Padding = 0,
            Content = body
        };

        SizeChanged += (_, _) => UpdateSheetSize();
    }

    //--------------------------------------------------------------------------------
    // Layout
    //--------------------------------------------------------------------------------

    private void UpdateSheetSize()
    {
        if (!attached || (Width <= 0) || (Height <= 0))
        {
            return;
        }

        MeasureSheet();
        if (pendingOpen)
        {
            pendingOpen = false;
            sheet.TranslationY = SheetHeight;
            MoveTo(HalfY);
        }
        else if (!sheet.AnimationIsRunning(AnimationName))
        {
            sheet.TranslationY = IsOpen ? (expanded ? 0 : HalfY) : SheetHeight;
        }
    }

    private void MeasureSheet()
    {
        SheetHeight = Math.Min(Height * ExpandedRatio, body.Measure(Width, double.PositiveInfinity).Height);
        sheet.HeightRequest = SheetHeight;
    }

    private static RoundRectangle CreateShape(double radius) => new() { CornerRadius = new CornerRadius(radius, radius, 0, 0) };

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private void OnIsOpenChanged(bool open)
    {
        if (open)
        {
            if ((SheetContent is null) && (SheetTemplate is not null))
            {
                SheetContent = (View)SheetTemplate.CreateContent();
            }

            if (!attached)
            {
                attached = true;
                Children.Add(backdrop);
                Children.Add(sheet);
            }

            expanded = false;
            IsVisible = true;
            if ((Width > 0) && (Height > 0))
            {
                MeasureSheet();
                sheet.TranslationY = SheetHeight;
                MoveTo(HalfY);
            }
            else
            {
                pendingOpen = true;
            }
        }
        else
        {
            pendingOpen = false;
            sheet.AbortAnimation(AnimationName);
            var start = sheet.TranslationY;
            var end = SheetHeight;
            var startOpacity = backdrop.Opacity;
            var fling = closeDuration is not null;
            var duration = closeDuration ?? Duration;
            closeDuration = null;
            sheet.Animate(
                AnimationName,
                v =>
                {
                    sheet.TranslationY = start + ((end - start) * v);
                    backdrop.Opacity = startOpacity * (1 - v);
                },
                AnimationRate,
                duration,
                fling ? Easing.SinOut : Easing.CubicIn,
                (_, _) => IsVisible = IsOpen);
        }
    }

    private void MoveTo(double y, uint duration = Duration, Easing? easing = null)
    {
        expanded = y < HalfY;
        sheet.AbortAnimation(AnimationName);
        var start = sheet.TranslationY;
        var startOpacity = backdrop.Opacity;
        var endOpacity = BackdropOpacity * (1 - (y / SheetHeight));
        sheet.Animate(
            AnimationName,
            v =>
            {
                sheet.TranslationY = start + ((y - start) * v);
                backdrop.Opacity = startOpacity + ((endOpacity - startOpacity) * v);
            },
            AnimationRate,
            duration,
            easing ?? Easing.CubicOut);
    }

    //--------------------------------------------------------------------------------
    // Drag
    //--------------------------------------------------------------------------------

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panning = IsOpen;
                if (!panning)
                {
                    break;
                }

                sheet.AbortAnimation(AnimationName);
                panStart = sheet.TranslationY;
                panTotal = 0;
                panVelocity = 0;
                panDirection = 0;
                panTimestamp = Environment.TickCount64;
                break;
            case GestureStatus.Running:
                if (!panning)
                {
                    break;
                }

                var delta = e.TotalY - panTotal;
                var now = Environment.TickCount64;
                var elapsed = now - panTimestamp;
                if ((delta != 0) && (elapsed > 0))
                {
                    var velocity = delta / elapsed;
                    panVelocity = (panVelocity == 0) || (Math.Sign(velocity) != Math.Sign(panVelocity)) ? velocity : (panVelocity + velocity) / 2;
                    panTimestamp = now;
                }

                if (Math.Abs(delta) >= DirectionDistance)
                {
                    panDirection = Math.Sign(delta);
                }

                panTotal = e.TotalY;
                var y = Math.Clamp(panStart + e.TotalY, 0, SheetHeight);
                sheet.TranslationY = y;
                backdrop.Opacity = BackdropOpacity * (1 - (y / SheetHeight));
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (panning)
                {
                    panning = false;
                    Settle();
                }

                break;
        }
    }

    private void Settle()
    {
        var y = sheet.TranslationY;
        var velocity = Environment.TickCount64 - panTimestamp > PauseInterval ? 0 : panVelocity;
        if ((velocity >= SwipeVelocity) || ((velocity >= 0) && (panDirection > 0) && (panTotal >= SwipeDistance)))
        {
            if (y < HalfY - SwipeDistance)
            {
                MoveTo(HalfY, FlingDuration(HalfY - y, velocity), Easing.SinOut);
            }
            else
            {
                closeDuration = FlingDuration(SheetHeight - y, velocity);
                IsOpen = false;
            }

            return;
        }

        if ((velocity <= -SwipeVelocity) || ((velocity <= 0) && (panDirection < 0) && (panTotal <= -SwipeDistance)))
        {
            var target = y > HalfY + SwipeDistance ? HalfY : 0;
            MoveTo(target, FlingDuration(y - target, -velocity), Easing.SinOut);
            return;
        }

        if (y > (HalfY + SheetHeight) / 2)
        {
            IsOpen = false;
            return;
        }

        MoveTo(y < HalfY / 2 ? 0 : HalfY);
    }

    private static uint FlingDuration(double distance, double velocity) =>
        velocity > 0 ? (uint)Math.Clamp(FlingSlope * distance / velocity, MinFlingDuration, MaxFlingDuration) : Duration;
}
