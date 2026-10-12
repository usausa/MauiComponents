namespace MauiComponents.Controls;

public sealed partial class SideDrawer : Grid
{
    private const string AnimationName = "DrawerMove";

    private const uint AnimationRate = 16;

    private const uint MinFlingDuration = 80;

    private const uint MaxFlingDuration = 400;

    private const double FlingSlope = Math.PI / 2;

    private const double SwipeDistance = 16;

    private const double SwipeVelocity = 0.2;

    private const double DirectionDistance = 1;

    private const long PauseInterval = 100;

    private const float ShadowOpacity = 0.25f;

    private const float ShadowRadius = 12;

    private static readonly Color EdgeColor = Color.FromRgba(0, 0, 0, 0.01);

    private static readonly Point ShadowOffset = new(2, 0);

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private readonly Grid edge;

    private readonly BoxView backdrop;

    private readonly Grid panel;

    private double panStart;

    private double panTotal;

    private double panVelocity;

    private int panDirection;

    private long panTimestamp;

    private uint? flingDuration;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(SideDrawer),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((SideDrawer)bindable).OnIsOpenChanged((bool)newValue));

    public static readonly BindableProperty DrawerContentProperty = BindableProperty.Create(
        nameof(DrawerContent),
        typeof(View),
        typeof(SideDrawer),
        propertyChanged: static (bindable, _, newValue) => ((SideDrawer)bindable).SetDrawerContent((View?)newValue));

    public static readonly BindableProperty DrawerWidthProperty = BindableProperty.Create(
        nameof(DrawerWidth),
        typeof(double),
        typeof(SideDrawer),
        280d,
        propertyChanged: static (bindable, _, _) => ((SideDrawer)bindable).UpdatePanelSize());

    public static readonly BindableProperty EdgeSwipeEnabledProperty = BindableProperty.Create(
        nameof(EdgeSwipeEnabled),
        typeof(bool),
        typeof(SideDrawer),
        true,
        propertyChanged: static (bindable, _, newValue) => ((SideDrawer)bindable).edge.IsVisible = (bool)newValue);

    public static readonly BindableProperty EdgeWidthProperty = BindableProperty.Create(
        nameof(EdgeWidth),
        typeof(double),
        typeof(SideDrawer),
        24d,
        propertyChanged: static (bindable, _, newValue) => ((SideDrawer)bindable).edge.WidthRequest = (double)newValue);

    public static readonly BindableProperty DrawerBackgroundColorProperty = BindableProperty.Create(
        nameof(DrawerBackgroundColor),
        typeof(Color),
        typeof(SideDrawer),
        Colors.White,
        propertyChanged: static (bindable, _, newValue) => ((SideDrawer)bindable).panel.BackgroundColor = (Color)newValue);

    public static readonly BindableProperty BackdropOpacityProperty = BindableProperty.Create(
        nameof(BackdropOpacity),
        typeof(double),
        typeof(SideDrawer),
        0.4d);

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(uint),
        typeof(SideDrawer),
        150u);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public View? DrawerContent
    {
        get => (View?)GetValue(DrawerContentProperty);
        set => SetValue(DrawerContentProperty, value);
    }

    public double DrawerWidth
    {
        get => (double)GetValue(DrawerWidthProperty);
        set => SetValue(DrawerWidthProperty, value);
    }

    public bool EdgeSwipeEnabled
    {
        get => (bool)GetValue(EdgeSwipeEnabledProperty);
        set => SetValue(EdgeSwipeEnabledProperty, value);
    }

    public double EdgeWidth
    {
        get => (double)GetValue(EdgeWidthProperty);
        set => SetValue(EdgeWidthProperty, value);
    }

    public Color DrawerBackgroundColor
    {
        get => (Color)GetValue(DrawerBackgroundColorProperty);
        set => SetValue(DrawerBackgroundColorProperty, value);
    }

    public double BackdropOpacity
    {
        get => (double)GetValue(BackdropOpacityProperty);
        set => SetValue(BackdropOpacityProperty, value);
    }

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public SideDrawer()
    {
        InputTransparent = true;
        CascadeInputTransparent = false;

        edge = new Grid
        {
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = EdgeWidth,
            BackgroundColor = EdgeColor
        };
        var edgePan = new PanGestureRecognizer();
        edgePan.PanUpdated += OnPanUpdated;
        edge.GestureRecognizers.Add(edgePan);
        edge.HandlerChanged += (_, _) => UpdateGestureExclusion();
        edge.SizeChanged += (_, _) => UpdateGestureExclusion();

        backdrop = new BoxView { Color = Colors.Black, BackgroundColor = Colors.Transparent, Opacity = 0, IsVisible = false };
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => IsOpen = false;
        backdrop.GestureRecognizers.Add(tap);
        var backdropPan = new PanGestureRecognizer();
        backdropPan.PanUpdated += OnPanUpdated;
        backdrop.GestureRecognizers.Add(backdropPan);

        panel = CreatePanel();
        panel.HorizontalOptions = LayoutOptions.Start;
        panel.WidthRequest = DrawerWidth;
        panel.BackgroundColor = DrawerBackgroundColor;
        panel.TranslationX = -DrawerWidth;
        panel.IsVisible = false;
        panel.Shadow = new Shadow { Brush = Brush.Black, Opacity = ShadowOpacity, Radius = ShadowRadius, Offset = ShadowOffset };

        Children.Add(edge);
        Children.Add(backdrop);
        Children.Add(panel);
    }

    //--------------------------------------------------------------------------------
    // Platform
    //--------------------------------------------------------------------------------

    partial void UpdateGestureExclusion();

    private partial Grid CreatePanel();

    //--------------------------------------------------------------------------------
    // Content
    //--------------------------------------------------------------------------------

    private void SetDrawerContent(View? view)
    {
        panel.Children.Clear();
        if (view is not null)
        {
            panel.Children.Add(view);
        }
    }

    private void UpdatePanelSize()
    {
        panel.WidthRequest = DrawerWidth;
        if (!IsOpen)
        {
            panel.TranslationX = -DrawerWidth;
        }
    }

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private void OnIsOpenChanged(bool open)
    {
        if (open)
        {
            backdrop.IsVisible = true;
            panel.IsVisible = true;
        }

        var duration = flingDuration ?? Duration;
        var easing = flingDuration is null ? null : Easing.SinOut;
        flingDuration = null;
        MoveTo(open ? 0 : -DrawerWidth, open, duration, easing);
    }

    private void MoveTo(double x, bool open, uint duration, Easing? easing)
    {
        panel.AbortAnimation(AnimationName);
        var start = panel.TranslationX;
        var startOpacity = backdrop.Opacity;
        var endOpacity = open ? BackdropOpacity : 0;
        if (duration > 0)
        {
            panel.Animate(
                AnimationName,
                v =>
                {
                    panel.TranslationX = start + ((x - start) * v);
                    backdrop.Opacity = startOpacity + ((endOpacity - startOpacity) * v);
                },
                AnimationRate,
                duration,
                easing ?? (open ? Easing.CubicOut : Easing.CubicIn),
                (_, _) => UpdateVisibility());
        }
        else
        {
            panel.TranslationX = x;
            backdrop.Opacity = endOpacity;
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        backdrop.IsVisible = IsOpen;
        panel.IsVisible = IsOpen;
    }

    //--------------------------------------------------------------------------------
    // Drag
    //--------------------------------------------------------------------------------

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                StartDrag();
                break;
            case GestureStatus.Running:
                MoveDrag(e.TotalX);
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                Settle();
                break;
        }
    }

    private void StartDrag()
    {
        panel.AbortAnimation(AnimationName);
        panStart = panel.TranslationX;
        panTotal = 0;
        panVelocity = 0;
        panDirection = 0;
        panTimestamp = Environment.TickCount64;
        backdrop.IsVisible = true;
        panel.IsVisible = true;
    }

    private void MoveDrag(double totalX)
    {
        var delta = totalX - panTotal;
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

        panTotal = totalX;
        var x = Math.Clamp(panStart + totalX, -DrawerWidth, 0);
        panel.TranslationX = x;
        backdrop.Opacity = BackdropOpacity * (1 + (x / DrawerWidth));
    }

    private void Settle()
    {
        var x = panel.TranslationX;
        var velocity = Environment.TickCount64 - panTimestamp > PauseInterval ? 0 : panVelocity;
        bool open;
        if ((velocity >= SwipeVelocity) || ((velocity >= 0) && (panDirection > 0) && (panTotal >= SwipeDistance)))
        {
            open = true;
        }
        else if ((velocity <= -SwipeVelocity) || ((velocity <= 0) && (panDirection < 0) && (panTotal <= -SwipeDistance)))
        {
            open = false;
        }
        else
        {
            open = x > -DrawerWidth / 2;
        }

        var duration = FlingDuration(open ? -x : x + DrawerWidth, open ? velocity : -velocity, Duration);
        if (open == IsOpen)
        {
            MoveTo(open ? 0 : -DrawerWidth, open, duration, Easing.SinOut);
        }
        else
        {
            flingDuration = duration;
            IsOpen = open;
        }
    }

    private static uint FlingDuration(double distance, double velocity, uint duration) =>
        velocity > 0 ? (uint)Math.Clamp(FlingSlope * distance / velocity, MinFlingDuration, MaxFlingDuration) : duration;
}
