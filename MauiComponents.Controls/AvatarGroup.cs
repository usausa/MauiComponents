namespace MauiComponents.Controls;

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

using Microsoft.Maui.Controls.Shapes;

public sealed class AvatarGroup : ContentView
{
    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(AvatarGroup),
        propertyChanged: static (bindable, oldValue, newValue) => ((AvatarGroup)bindable).OnItemsSourceChanged(oldValue as IEnumerable, newValue as IEnumerable));

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(AvatarGroup),
        propertyChanged: Rebuild);

    public static readonly BindableProperty MaxDisplayedProperty = BindableProperty.Create(
        nameof(MaxDisplayed),
        typeof(int),
        typeof(AvatarGroup),
        3,
        propertyChanged: Rebuild);

    public static readonly BindableProperty OverlapProperty = BindableProperty.Create(
        nameof(Overlap),
        typeof(double),
        typeof(AvatarGroup),
        10d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty ShowCountProperty = BindableProperty.Create(
        nameof(ShowCount),
        typeof(bool),
        typeof(AvatarGroup),
        true,
        propertyChanged: Rebuild);

    public static readonly BindableProperty AvatarSizeProperty = BindableProperty.Create(
        nameof(AvatarSize),
        typeof(double),
        typeof(AvatarGroup),
        32d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
        nameof(StrokeColor),
        typeof(Color),
        typeof(AvatarGroup),
        Colors.White,
        propertyChanged: Rebuild);

    public static readonly BindableProperty CountBackgroundColorProperty = BindableProperty.Create(
        nameof(CountBackgroundColor),
        typeof(Color),
        typeof(AvatarGroup),
        Color.FromArgb("#E0E0E0"),
        propertyChanged: Rebuild);

    public static readonly BindableProperty CountTextColorProperty = BindableProperty.Create(
        nameof(CountTextColor),
        typeof(Color),
        typeof(AvatarGroup),
        Color.FromArgb("#455A64"),
        propertyChanged: Rebuild);

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public int MaxDisplayed
    {
        get => (int)GetValue(MaxDisplayedProperty);
        set => SetValue(MaxDisplayedProperty, value);
    }

    public double Overlap
    {
        get => (double)GetValue(OverlapProperty);
        set => SetValue(OverlapProperty, value);
    }

    public bool ShowCount
    {
        get => (bool)GetValue(ShowCountProperty);
        set => SetValue(ShowCountProperty, value);
    }

    public double AvatarSize
    {
        get => (double)GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }

    public Color StrokeColor
    {
        get => (Color)GetValue(StrokeColorProperty);
        set => SetValue(StrokeColorProperty, value);
    }

    public Color CountBackgroundColor
    {
        get => (Color)GetValue(CountBackgroundColorProperty);
        set => SetValue(CountBackgroundColorProperty, value);
    }

    public Color CountTextColor
    {
        get => (Color)GetValue(CountTextColorProperty);
        set => SetValue(CountTextColorProperty, value);
    }

    private const double StrokeThickness = 2d;

    private readonly OverlapPanel panel = new() { ReverseZIndex = true };

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public AvatarGroup()
    {
        Content = panel;
        BuildContent();
    }

    //--------------------------------------------------------------------------------
    // Source
    //--------------------------------------------------------------------------------

    private static void Rebuild(BindableObject bindable, object oldValue, object newValue)
    {
        ((AvatarGroup)bindable).BuildContent();
    }

    private void OnItemsSourceChanged(IEnumerable? oldValue, IEnumerable? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= OnCollectionChanged;
        }

        if (newValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += OnCollectionChanged;
        }

        BuildContent();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        BuildContent();
    }

    //--------------------------------------------------------------------------------
    // Content
    //--------------------------------------------------------------------------------

    private void BuildContent()
    {
        panel.Clear();
        panel.OffsetX = AvatarSize - Overlap;

        var items = ItemsSource?.Cast<object>().ToList() ?? [];
        var shown = Math.Clamp(MaxDisplayed, 0, items.Count);
        for (var i = 0; i < shown; i++)
        {
            panel.Add(CreateItem(items[i]));
        }

        var rest = items.Count - shown;
        if ((rest > 0) && ShowCount)
        {
            panel.Add(CreateCount(rest));
        }
    }

    private View CreateItem(object item)
    {
        if (ItemTemplate?.CreateContent() is View view)
        {
            view.BindingContext = item;
            return view;
        }

        var border = CreateCircle();
        border.Content = new Image
        {
            Aspect = Aspect.AspectFill,
            Source = item switch
            {
                ImageSource source => source,
                string file => ImageSource.FromFile(file),
                _ => null
            }
        };
        return border;
    }

    private Border CreateCount(int rest)
    {
        var border = CreateCircle();
        border.BackgroundColor = CountBackgroundColor;
        border.Content = new Label
        {
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            TextColor = CountTextColor,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Text = "+" + rest.ToString(CultureInfo.InvariantCulture)
        };
        return border;
    }

    private Border CreateCircle() =>
        new()
        {
            WidthRequest = AvatarSize,
            HeightRequest = AvatarSize,
            Stroke = new SolidColorBrush(StrokeColor),
            StrokeThickness = StrokeThickness,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(AvatarSize / 2d) }
        };
}
