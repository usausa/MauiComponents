namespace MauiComponents.Controls;

using System.Globalization;
using System.Text;

public sealed class StepIndicator : ContentView
{
    private const double CountMargin = 10;

    private static readonly CompositeFormat CountFormat = CompositeFormat.Parse("{0} / {1}");

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty CurrentStepProperty = BindableProperty.Create(
        nameof(CurrentStep),
        typeof(int),
        typeof(StepIndicator),
        1,
        propertyChanged: Rebuild);

    public static readonly BindableProperty TotalStepsProperty = BindableProperty.Create(
        nameof(TotalSteps),
        typeof(int),
        typeof(StepIndicator),
        3,
        propertyChanged: Rebuild);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(StepIndicator),
        Color.FromArgb("#2196F3"),
        propertyChanged: Rebuild);

    public static readonly BindableProperty UpcomingColorProperty = BindableProperty.Create(
        nameof(UpcomingColor),
        typeof(Color),
        typeof(StepIndicator),
        Color.FromArgb("#E0E0E0"),
        propertyChanged: Rebuild);

    public static readonly BindableProperty DotSizeProperty = BindableProperty.Create(
        nameof(DotSize),
        typeof(double),
        typeof(StepIndicator),
        10d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty CurrentDotWidthProperty = BindableProperty.Create(
        nameof(CurrentDotWidth),
        typeof(double),
        typeof(StepIndicator),
        24d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty DotSpacingProperty = BindableProperty.Create(
        nameof(DotSpacing),
        typeof(double),
        typeof(StepIndicator),
        6d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty CountFontSizeProperty = BindableProperty.Create(
        nameof(CountFontSize),
        typeof(double),
        typeof(StepIndicator),
        14d,
        propertyChanged: Rebuild);

    public static readonly BindableProperty CountTextColorProperty = BindableProperty.Create(
        nameof(CountTextColor),
        typeof(Color),
        typeof(StepIndicator),
        Color.FromArgb("#546E7A"),
        propertyChanged: Rebuild);

    public static readonly BindableProperty IsCountVisibleProperty = BindableProperty.Create(
        nameof(IsCountVisible),
        typeof(bool),
        typeof(StepIndicator),
        true,
        propertyChanged: Rebuild);

    public int CurrentStep
    {
        get => (int)GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }

    public int TotalSteps
    {
        get => (int)GetValue(TotalStepsProperty);
        set => SetValue(TotalStepsProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public Color UpcomingColor
    {
        get => (Color)GetValue(UpcomingColorProperty);
        set => SetValue(UpcomingColorProperty, value);
    }

    public double DotSize
    {
        get => (double)GetValue(DotSizeProperty);
        set => SetValue(DotSizeProperty, value);
    }

    public double CurrentDotWidth
    {
        get => (double)GetValue(CurrentDotWidthProperty);
        set => SetValue(CurrentDotWidthProperty, value);
    }

    public double DotSpacing
    {
        get => (double)GetValue(DotSpacingProperty);
        set => SetValue(DotSpacingProperty, value);
    }

    public double CountFontSize
    {
        get => (double)GetValue(CountFontSizeProperty);
        set => SetValue(CountFontSizeProperty, value);
    }

    public Color CountTextColor
    {
        get => (Color)GetValue(CountTextColorProperty);
        set => SetValue(CountTextColorProperty, value);
    }

    public bool IsCountVisible
    {
        get => (bool)GetValue(IsCountVisibleProperty);
        set => SetValue(IsCountVisibleProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public StepIndicator()
    {
        BuildContent();
    }

    //--------------------------------------------------------------------------------
    // Content
    //--------------------------------------------------------------------------------

    private static void Rebuild(BindableObject bindable, object oldValue, object newValue)
    {
        ((StepIndicator)bindable).BuildContent();
    }

    private void BuildContent()
    {
        var layout = new HorizontalStackLayout
        {
            Spacing = DotSpacing,
            HorizontalOptions = LayoutOptions.Center
        };

        for (var i = 1; i <= TotalSteps; i++)
        {
            layout.Children.Add(new BoxView
            {
                WidthRequest = i == CurrentStep ? CurrentDotWidth : DotSize,
                HeightRequest = DotSize,
                CornerRadius = DotSize / 2,
                Color = i <= CurrentStep ? AccentColor : UpcomingColor,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center
            });
        }

        if (IsCountVisible)
        {
            layout.Children.Add(new Label
            {
                Margin = new Thickness(CountMargin, 0, 0, 0),
                FontSize = CountFontSize,
                FontAttributes = FontAttributes.Bold,
                TextColor = CountTextColor,
                VerticalOptions = LayoutOptions.Center,
                Text = String.Format(CultureInfo.CurrentCulture, CountFormat, CurrentStep, TotalSteps)
            });
        }

        Content = layout;
    }
}
