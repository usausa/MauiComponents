namespace MauiComponents.Controls;

[ContentProperty(nameof(Skeleton))]
public sealed class ShimmerView : ContentView
{
    private const string AnimationName = "ShimmerWave";

    private const uint AnimationRate = 16;

    private static readonly Point WaveStartPoint = new(0, 0);

    private static readonly Point WaveEndPoint = new(1, 1);

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private readonly Grid root;

    private readonly GraphicsView canvas;

    private PathF? shapes;

    private double position;

    private bool running;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty SkeletonProperty = BindableProperty.Create(
        nameof(Skeleton),
        typeof(View),
        typeof(ShimmerView),
        propertyChanged: static (bindable, oldValue, newValue) => ((ShimmerView)bindable).SetSkeleton((View?)oldValue, (View?)newValue));

    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
        nameof(IsActive),
        typeof(bool),
        typeof(ShimmerView),
        true,
        propertyChanged: static (bindable, _, _) => ((ShimmerView)bindable).UpdateAnimation());

    public static readonly BindableProperty SurfaceColorProperty = BindableProperty.Create(
        nameof(SurfaceColor),
        typeof(Color),
        typeof(ShimmerView),
        Color.FromArgb("#FFFBFE"),
        propertyChanged: static (bindable, _, newValue) => ((ShimmerView)bindable).root.BackgroundColor = (Color)newValue);

    public static readonly BindableProperty FillProperty = BindableProperty.Create(
        nameof(Fill),
        typeof(Color),
        typeof(ShimmerView),
        Color.FromArgb("#F7F2FB"),
        propertyChanged: static (bindable, _, _) => ((ShimmerView)bindable).canvas.Invalidate());

    public static readonly BindableProperty WaveColorProperty = BindableProperty.Create(
        nameof(WaveColor),
        typeof(Color),
        typeof(ShimmerView),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((ShimmerView)bindable).canvas.Invalidate());

    public static readonly BindableProperty WaveWidthProperty = BindableProperty.Create(
        nameof(WaveWidth),
        typeof(double),
        typeof(ShimmerView),
        200d,
        propertyChanged: static (bindable, _, _) => ((ShimmerView)bindable).RestartAnimation());

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(uint),
        typeof(ShimmerView),
        1000u,
        propertyChanged: static (bindable, _, _) => ((ShimmerView)bindable).RestartAnimation());

    public View? Skeleton
    {
        get => (View?)GetValue(SkeletonProperty);
        set => SetValue(SkeletonProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public Color SurfaceColor
    {
        get => (Color)GetValue(SurfaceColorProperty);
        set => SetValue(SurfaceColorProperty, value);
    }

    public Color Fill
    {
        get => (Color)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public Color WaveColor
    {
        get => (Color)GetValue(WaveColorProperty);
        set => SetValue(WaveColorProperty, value);
    }

    public double WaveWidth
    {
        get => (double)GetValue(WaveWidthProperty);
        set => SetValue(WaveWidthProperty, value);
    }

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public ShimmerView()
    {
        canvas = new GraphicsView { Drawable = new WaveDrawable(this), InputTransparent = true };
        root = [canvas];
        root.BackgroundColor = SurfaceColor;
        Content = root;

        Loaded += (_, _) => UpdateAnimation();
        Unloaded += (_, _) => UpdateAnimation();
        SizeChanged += (_, _) =>
        {
            shapes = null;
            RestartAnimation();
        };
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == IsVisibleProperty.PropertyName)
            {
                UpdateAnimation();
            }
        };
    }

    //--------------------------------------------------------------------------------
    // Skeleton
    //--------------------------------------------------------------------------------

    private void SetSkeleton(View? oldView, View? newView)
    {
        if (oldView is not null)
        {
            root.Remove(oldView);
        }

        if (newView is not null)
        {
            root.Insert(0, newView);
        }

        shapes = null;
        UpdateAnimation();
    }

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private void RestartAnimation()
    {
        this.AbortAnimation(AnimationName);
        running = false;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Skeleton?.Opacity = IsActive ? 0 : 1;

        var run = IsActive && IsVisible && IsLoaded && (Width > 0);
        if (run && !running)
        {
            running = true;
            var end = 1 + (WaveWidth / Width);
            this.Animate(
                AnimationName,
                v =>
                {
                    position = v;
                    canvas.Invalidate();
                },
                0,
                end,
                AnimationRate,
                Duration,
                Easing.Linear,
                null,
                () => running);
        }
        else if (!run && running)
        {
            running = false;
            this.AbortAnimation(AnimationName);
        }

        canvas.Invalidate();
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private void Draw(ICanvas target, RectF dirtyRect)
    {
        if (IsActive && (Skeleton is not null) && (dirtyRect.Width > 0))
        {
            shapes ??= CreateShapes(Skeleton);
            var wave = (float)Math.Clamp(WaveWidth / dirtyRect.Width, 0, 1);
            var right = (float)position;
            var paint = new LinearGradientPaint
            {
                StartPoint = WaveStartPoint,
                EndPoint = WaveEndPoint,
                GradientStops =
                [
                    new PaintGradientStop(Math.Clamp(right - wave, 0, 1), Fill),
                    new PaintGradientStop(Math.Clamp(right - (wave / 2), 0, 1), WaveColor),
                    new PaintGradientStop(Math.Clamp(right, 0, 1), Fill)
                ]
            };
            target.SetFillPaint(paint, shapes.Bounds);
            target.FillPath(shapes);
        }
    }

    private static PathF CreateShapes(View skeleton)
    {
        var path = new PathF();
        AddShapes(path, skeleton, skeleton.X, skeleton.Y);
        return path;
    }

    private static void AddShapes(PathF path, VisualElement element, double x, double y)
    {
        if (element is BoxView box)
        {
            path.AppendRoundedRectangle(
                (float)x,
                (float)y,
                (float)box.Width,
                (float)box.Height,
                (float)box.CornerRadius.TopLeft,
                (float)box.CornerRadius.TopRight,
                (float)box.CornerRadius.BottomLeft,
                (float)box.CornerRadius.BottomRight);
        }
        else
        {
            foreach (var child in ((IVisualTreeElement)element).GetVisualChildren().OfType<VisualElement>())
            {
                AddShapes(path, child, x + child.X, y + child.Y);
            }
        }
    }

    private sealed class WaveDrawable : IDrawable
    {
        private readonly ShimmerView owner;

        public WaveDrawable(ShimmerView owner)
        {
            this.owner = owner;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect) => owner.Draw(canvas, dirtyRect);
    }
}
