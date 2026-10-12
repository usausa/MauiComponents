namespace MauiComponents.Controls;

using System.Collections;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

public sealed class ChipGroup : ContentView
{
    private const double TextPadding = 15;

    private const double TextHeightRate = 1.3;

    private const double HeightPadding = 13;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(ChipGroup),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(ChipGroup),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ItemStyleProperty = BindableProperty.Create(
        nameof(ItemStyle),
        typeof(Style),
        typeof(ChipGroup),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(ChipGroup),
        null,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty WrapProperty = BindableProperty.Create(
        nameof(Wrap),
        typeof(bool),
        typeof(ChipGroup),
        false,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(ChipGroup),
        4d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipPaddingProperty = BindableProperty.Create(
        nameof(ChipPadding),
        typeof(Thickness),
        typeof(ChipGroup),
        Thickness.Zero,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipTextSizeProperty = BindableProperty.Create(
        nameof(ChipTextSize),
        typeof(double),
        typeof(ChipGroup),
        14d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipCornerRadiusProperty = BindableProperty.Create(
        nameof(ChipCornerRadius),
        typeof(double),
        typeof(ChipGroup),
        16d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipBackgroundProperty = BindableProperty.Create(
        nameof(ChipBackground),
        typeof(Color),
        typeof(ChipGroup),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty ChipStrokeProperty = BindableProperty.Create(
        nameof(ChipStroke),
        typeof(Color),
        typeof(ChipGroup),
        Colors.LightGray,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty ChipStrokeThicknessProperty = BindableProperty.Create(
        nameof(ChipStrokeThickness),
        typeof(double),
        typeof(ChipGroup),
        1d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipTextColorProperty = BindableProperty.Create(
        nameof(ChipTextColor),
        typeof(Color),
        typeof(ChipGroup),
        Colors.Black,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty SelectedChipBackgroundProperty = BindableProperty.Create(
        nameof(SelectedChipBackground),
        typeof(Color),
        typeof(ChipGroup),
        Colors.Blue,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty SelectedChipTextColorProperty = BindableProperty.Create(
        nameof(SelectedChipTextColor),
        typeof(Color),
        typeof(ChipGroup),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

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

    public Style? ItemStyle
    {
        get => (Style?)GetValue(ItemStyleProperty);
        set => SetValue(ItemStyleProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public Thickness ChipPadding
    {
        get => (Thickness)GetValue(ChipPaddingProperty);
        set => SetValue(ChipPaddingProperty, value);
    }

    public double ChipTextSize
    {
        get => (double)GetValue(ChipTextSizeProperty);
        set => SetValue(ChipTextSizeProperty, value);
    }

    public double ChipCornerRadius
    {
        get => (double)GetValue(ChipCornerRadiusProperty);
        set => SetValue(ChipCornerRadiusProperty, value);
    }

    public Color ChipBackground
    {
        get => (Color)GetValue(ChipBackgroundProperty);
        set => SetValue(ChipBackgroundProperty, value);
    }

    public Color ChipStroke
    {
        get => (Color)GetValue(ChipStrokeProperty);
        set => SetValue(ChipStrokeProperty, value);
    }

    public double ChipStrokeThickness
    {
        get => (double)GetValue(ChipStrokeThicknessProperty);
        set => SetValue(ChipStrokeThicknessProperty, value);
    }

    public Color ChipTextColor
    {
        get => (Color)GetValue(ChipTextColorProperty);
        set => SetValue(ChipTextColorProperty, value);
    }

    public Color SelectedChipBackground
    {
        get => (Color)GetValue(SelectedChipBackgroundProperty);
        set => SetValue(SelectedChipBackgroundProperty, value);
    }

    public Color SelectedChipTextColor
    {
        get => (Color)GetValue(SelectedChipTextColorProperty);
        set => SetValue(SelectedChipTextColorProperty, value);
    }

    public ICommand SelectCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public ChipGroup()
    {
        SelectCommand = new Command<object?>(x => SelectedItem = x);
    }

    //--------------------------------------------------------------------------------
    // Chip
    //--------------------------------------------------------------------------------

    private void CreateChips()
    {
        Layout layout = Wrap
            ? new FlexLayout
            {
                Wrap = FlexWrap.Wrap,
                Direction = FlexDirection.Row,
                JustifyContent = FlexJustify.Start,
                AlignItems = FlexAlignItems.Start,
                AlignContent = FlexAlignContent.Start
            }
            : new HorizontalStackLayout { Spacing = Spacing };
        if (ItemsSource is not null)
        {
            foreach (var item in ItemsSource)
            {
                var chip = CreateChip(item);
                if (Wrap)
                {
                    FlexLayout.SetShrink(chip, 0);
                }

                layout.Children.Add(chip);
            }
        }

        Content = layout;
        UpdateChips();
    }

    private Border CreateChip(object? item)
    {
        var label = ItemTemplate?.CreateContent() as Label ?? new Label { Text = item?.ToString() ?? string.Empty };
        label.BindingContext = item;
        label.Padding = new Thickness(TextPadding, 0);
        label.FontSize = ChipTextSize;
        label.HorizontalTextAlignment = TextAlignment.Center;
        label.VerticalTextAlignment = TextAlignment.Center;
        if (ItemStyle is not null)
        {
            label.Style = ItemStyle;
        }

        var chip = new Border
        {
            Margin = ChipPadding,
            Padding = 0,
            HeightRequest = (ChipTextSize * TextHeightRate) + HeightPadding,
            StrokeThickness = ChipStrokeThickness,
            StrokeShape = new RoundRectangle { CornerRadius = ChipCornerRadius },
            Content = label
        };
        chip.GestureRecognizers.Add(new TapGestureRecognizer { Command = SelectCommand, CommandParameter = item });
        return chip;
    }

    private void UpdateChips()
    {
        if (Content is Layout layout)
        {
            foreach (var chip in layout.Children.OfType<Border>())
            {
                if (chip.Content is Label label)
                {
                    var selected = Equals(label.BindingContext, SelectedItem);
                    chip.BackgroundColor = selected ? SelectedChipBackground : ChipBackground;
                    chip.Stroke = selected ? SelectedChipBackground : ChipStroke;
                    label.TextColor = selected ? SelectedChipTextColor : ChipTextColor;
                }
            }
        }
    }
}
