namespace MauiComponents.Controls;

using System.Collections;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

public sealed class SegmentedView : ContentView
{
    private const string AnimationName = "SegmentedViewSelection";

    private const uint Duration = 250;

    private const uint AnimationRate = 16;

    private const double StrokeThickness = 1;

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private readonly Border border;

    private readonly Grid grid;

    private readonly BoxView selection;

    private readonly List<Label> labels = [];

    private List<object?> items = [];

    private double position;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(SegmentedView),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty ItemStyleProperty = BindableProperty.Create(
        nameof(ItemStyle),
        typeof(Style),
        typeof(SegmentedView),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(SegmentedView),
        0,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).MoveSelection((int)newValue, true));

    public static readonly BindableProperty SegmentWidthProperty = BindableProperty.Create(
        nameof(SegmentWidth),
        typeof(double),
        typeof(SegmentedView),
        100d,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty SegmentHeightProperty = BindableProperty.Create(
        nameof(SegmentHeight),
        typeof(double),
        typeof(SegmentedView),
        36d,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).grid.HeightRequest = (double)newValue);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(SegmentedView),
        16d,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#1C1B1F"),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).UpdateColors());

    public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(
        nameof(SelectedTextColor),
        typeof(Color),
        typeof(SegmentedView),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).UpdateColors());

    public static readonly BindableProperty SelectedBackgroundProperty = BindableProperty.Create(
        nameof(SelectedBackground),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#6750A4"),
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).selection.Color = (Color)newValue);

    public static readonly BindableProperty StrokeProperty = BindableProperty.Create(
        nameof(Stroke),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#79747E"),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(SegmentedView),
        20d,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).border.StrokeShape = new RoundRectangle { CornerRadius = (double)newValue });

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public Style? ItemStyle
    {
        get => (Style?)GetValue(ItemStyleProperty);
        set => SetValue(ItemStyleProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public double SegmentWidth
    {
        get => (double)GetValue(SegmentWidthProperty);
        set => SetValue(SegmentWidthProperty, value);
    }

    public double SegmentHeight
    {
        get => (double)GetValue(SegmentHeightProperty);
        set => SetValue(SegmentHeightProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public Color SelectedBackground
    {
        get => (Color)GetValue(SelectedBackgroundProperty);
        set => SetValue(SelectedBackgroundProperty, value);
    }

    public Color Stroke
    {
        get => (Color)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public ICommand SelectCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public SegmentedView()
    {
        HorizontalOptions = LayoutOptions.Center;
        SelectCommand = new Command<object?>(Select);

        selection = new BoxView
        {
            Color = SelectedBackground,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Start,
            InputTransparent = true
        };
        grid = new Grid { HeightRequest = SegmentHeight };
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        grid.GestureRecognizers.Add(tap);
        border = new Border
        {
            Padding = 0,
            StrokeThickness = StrokeThickness,
            StrokeShape = new RoundRectangle { CornerRadius = CornerRadius },
            Content = grid
        };
        Content = border;
    }

    //--------------------------------------------------------------------------------
    // Segment
    //--------------------------------------------------------------------------------

    private void CreateSegments()
    {
        grid.Clear();
        grid.ColumnDefinitions.Clear();
        labels.Clear();
        border.Stroke = Stroke;

        items = ItemsSource?.Cast<object?>().ToList() ?? [];
        selection.WidthRequest = SegmentWidth;
        grid.Add(selection);
        for (var i = 0; i < items.Count; i++)
        {
            if (i > 0)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(StrokeThickness)));
                grid.Add(new BoxView { Color = Stroke, BackgroundColor = Colors.Transparent, InputTransparent = true }, (i * 2) - 1);
            }

            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(SegmentWidth)));
        }

        for (var i = 0; i < items.Count; i++)
        {
            var label = new Label
            {
                BindingContext = items[i],
                Text = items[i]?.ToString() ?? string.Empty,
                FontSize = FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            if (ItemStyle is not null)
            {
                label.Style = ItemStyle;
            }

            labels.Add(label);
            grid.Add(label, i * 2);
        }

        selection.IsVisible = items.Count > 0;
        MoveSelection(SelectedIndex, false);
    }

    //--------------------------------------------------------------------------------
    // Selection
    //--------------------------------------------------------------------------------

    private void Select(object? item)
    {
        var index = items.IndexOf(item);
        if (index >= 0)
        {
            SelectedIndex = index;
        }
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (e.GetPosition(grid) is not { } point)
        {
            return;
        }

        var step = SegmentWidth + StrokeThickness;
        var index = (int)Math.Floor(point.X / step);
        if ((index >= 0) && (index < labels.Count) && (point.X - (index * step) < SegmentWidth))
        {
            SelectedIndex = index;
        }
    }

    private void MoveSelection(int index, bool animated)
    {
        selection.AbortAnimation(AnimationName);
        if (animated && (labels.Count > 0))
        {
            var start = position;
            selection.Animate(AnimationName, v => SetPosition(start + ((index - start) * v)), AnimationRate, Duration, Easing.CubicOut);
        }
        else
        {
            SetPosition(index);
        }
    }

    private void SetPosition(double value)
    {
        position = value;
        selection.TranslationX = value * (SegmentWidth + StrokeThickness);
        UpdateColors();
    }

    private void UpdateColors()
    {
        for (var i = 0; i < labels.Count; i++)
        {
            var rate = Math.Clamp(1 - Math.Abs(position - i), 0, 1);
            labels[i].TextColor = new Color(
                (float)Lerp(TextColor.Red, SelectedTextColor.Red, rate),
                (float)Lerp(TextColor.Green, SelectedTextColor.Green, rate),
                (float)Lerp(TextColor.Blue, SelectedTextColor.Blue, rate),
                (float)Lerp(TextColor.Alpha, SelectedTextColor.Alpha, rate));
        }
    }

    private static double Lerp(double from, double to, double t) => from + ((to - from) * t);
}
