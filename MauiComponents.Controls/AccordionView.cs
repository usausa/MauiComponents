namespace MauiComponents.Controls;

using System.Windows.Input;

public enum AccordionExpandMode
{
    SingleOrNone,
    MultipleOrNone
}

public sealed class AccordionView : VerticalStackLayout
{
    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ExpandModeProperty = BindableProperty.Create(
        nameof(ExpandMode),
        typeof(AccordionExpandMode),
        typeof(AccordionView),
        AccordionExpandMode.MultipleOrNone);

    public AccordionExpandMode ExpandMode
    {
        get => (AccordionExpandMode)GetValue(ExpandModeProperty);
        set => SetValue(ExpandModeProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Expand
    //--------------------------------------------------------------------------------

    internal void OnItemExpanded(AccordionItem item)
    {
        if (ExpandMode == AccordionExpandMode.SingleOrNone)
        {
            foreach (var other in Children.OfType<AccordionItem>().Where(x => x != item))
            {
                other.IsExpanded = false;
            }
        }
    }
}

public sealed class AccordionItem : Grid
{
    private const string AnimationName = "AccordionItemExpand";

    private const uint AnimationRate = 16;

    private const double IconMargin = 8;

    private const double ExpandedRotation = 180;

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private readonly Grid headerRow;

    private readonly Label icon;

    private readonly BoxView line;

    private readonly ContentView body;

    private bool revealOnExpanded;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(View),
        typeof(AccordionItem),
        propertyChanged: static (bindable, oldValue, newValue) => ((AccordionItem)bindable).SetHeader((View?)oldValue, (View?)newValue));

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(AccordionItem),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).body.Content = (View?)newValue);

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        nameof(IsExpanded),
        typeof(bool),
        typeof(AccordionItem),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).OnIsExpandedChanged((bool)newValue));

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(AccordionItem),
        "▾",
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).icon.Text = (string)newValue);

    public static readonly BindableProperty IconFontFamilyProperty = BindableProperty.Create(
        nameof(IconFontFamily),
        typeof(string),
        typeof(AccordionItem),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).icon.FontFamily = (string?)newValue);

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(double),
        typeof(AccordionItem),
        24d,
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).icon.FontSize = (double)newValue);

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(AccordionItem),
        Color.FromArgb("#49454F"),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).icon.TextColor = (Color)newValue);

    public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
        nameof(LineColor),
        typeof(Color),
        typeof(AccordionItem),
        Color.FromArgb("#CAC4D0"),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).line.Color = (Color)newValue);

    public static readonly BindableProperty LineThicknessProperty = BindableProperty.Create(
        nameof(LineThickness),
        typeof(double),
        typeof(AccordionItem),
        0.5d,
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).line.HeightRequest = (double)newValue);

    public static readonly BindableProperty HeaderRowStyleProperty = BindableProperty.Create(
        nameof(HeaderRowStyle),
        typeof(Style),
        typeof(AccordionItem),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).headerRow.Style = (Style?)newValue);

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(uint),
        typeof(AccordionItem),
        250u);

    public View? Header
    {
        get => (View?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public View? Content
    {
        get => (View?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? IconFontFamily
    {
        get => (string?)GetValue(IconFontFamilyProperty);
        set => SetValue(IconFontFamilyProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    public double LineThickness
    {
        get => (double)GetValue(LineThicknessProperty);
        set => SetValue(LineThicknessProperty, value);
    }

    public Style? HeaderRowStyle
    {
        get => (Style?)GetValue(HeaderRowStyleProperty);
        set => SetValue(HeaderRowStyleProperty, value);
    }

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public ICommand ToggleCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public AccordionItem()
    {
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        ToggleCommand = new Command(Toggle);

        icon = new Label
        {
            Text = Icon,
            FontFamily = IconFontFamily,
            FontSize = IconSize,
            TextColor = IconColor,
            Margin = new Thickness(0, 0, IconMargin, 0),
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true
        };
        headerRow = new Grid
        {
            ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)]
        };
        Grid.SetColumn(icon, 1);
        headerRow.Children.Add(icon);
        var header = new Grid { headerRow };
        header.GestureRecognizers.Add(new TapGestureRecognizer { Command = ToggleCommand });

        line = new BoxView
        {
            HeightRequest = LineThickness,
            Color = LineColor,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Start,
            IsVisible = false,
            InputTransparent = true
        };

        body = new ContentView { IsClippedToBounds = true, IsVisible = false };

        Add(header);
        Add(line);
        this.Add(body, 0, 1);
    }

    //--------------------------------------------------------------------------------
    // Header
    //--------------------------------------------------------------------------------

    private void SetHeader(View? oldView, View? newView)
    {
        if (oldView is not null)
        {
            headerRow.Remove(oldView);
        }

        if (newView is not null)
        {
            headerRow.Add(newView);
        }
    }

    //--------------------------------------------------------------------------------
    // Expand
    //--------------------------------------------------------------------------------

    private void Toggle()
    {
        revealOnExpanded = !IsExpanded;
        IsExpanded = !IsExpanded;
    }

    private void OnIsExpandedChanged(bool expanded)
    {
        line.IsVisible = expanded;
        if (expanded && (Parent is AccordionView accordion))
        {
            accordion.OnItemExpanded(this);
        }

        this.AbortAnimation(AnimationName);
        if ((Duration > 0) && (Width > 0) && (Content is IView content))
        {
            var from = body.IsVisible ? body.Height : 0;
            var to = expanded ? content.Measure(Width, Double.PositiveInfinity).Height : 0;
            var fromRotation = icon.Rotation;
            var toRotation = expanded ? ExpandedRotation : 0d;
            body.HeightRequest = from;
            body.IsVisible = true;
            this.Animate(
                AnimationName,
                v =>
                {
                    body.HeightRequest = from + ((to - from) * v);
                    icon.Rotation = fromRotation + ((toRotation - fromRotation) * v);
                },
                AnimationRate,
                Duration,
                Easing.Linear,
                (_, canceled) =>
                {
                    if (!canceled)
                    {
                        body.IsVisible = expanded;
                        body.ClearValue(HeightRequestProperty);
                        if (expanded && revealOnExpanded)
                        {
                            revealOnExpanded = false;
                            Reveal();
                        }
                    }
                });
        }
        else
        {
            body.IsVisible = expanded;
            body.ClearValue(HeightRequestProperty);
            icon.Rotation = expanded ? ExpandedRotation : 0;
            if (expanded && revealOnExpanded && (Content is not null))
            {
                revealOnExpanded = false;
                SizeChanged += OnExpandedSizeChanged;
            }
        }
    }

    private void OnExpandedSizeChanged(object? sender, EventArgs e)
    {
        SizeChanged -= OnExpandedSizeChanged;
        Reveal();
    }

    private void Reveal()
    {
        var parent = Parent;
        while ((parent is not null) && (parent is not ScrollView))
        {
            parent = parent.Parent;
        }

        if (parent is ScrollView scroll)
        {
            _ = scroll.ScrollToAsync(this, ScrollToPosition.MakeVisible, true);
        }
    }
}
