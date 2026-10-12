namespace MauiComponents.Controls;

using Microsoft.Maui.Layouts;

public sealed class OverlapPanel : Layout
{
    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty OffsetXProperty = BindableProperty.Create(
        nameof(OffsetX),
        typeof(double),
        typeof(OverlapPanel),
        20d,
        propertyChanged: static (bindable, _, _) => ((OverlapPanel)bindable).InvalidateMeasure());

    public static readonly BindableProperty OffsetYProperty = BindableProperty.Create(
        nameof(OffsetY),
        typeof(double),
        typeof(OverlapPanel),
        0d,
        propertyChanged: static (bindable, _, _) => ((OverlapPanel)bindable).InvalidateMeasure());

    public static readonly BindableProperty ReverseZIndexProperty = BindableProperty.Create(
        nameof(ReverseZIndex),
        typeof(bool),
        typeof(OverlapPanel),
        false,
        propertyChanged: static (bindable, _, _) => ((OverlapPanel)bindable).UpdateZIndex());

    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public bool ReverseZIndex
    {
        get => (bool)GetValue(ReverseZIndexProperty);
        set => SetValue(ReverseZIndexProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Layout
    //--------------------------------------------------------------------------------

    protected override ILayoutManager CreateLayoutManager() => new OverlapLayoutManager(this);

    //--------------------------------------------------------------------------------
    // Children
    //--------------------------------------------------------------------------------

    protected override void OnAdd(int index, IView view)
    {
        base.OnAdd(index, view);
        UpdateZIndex();
    }

    protected override void OnInsert(int index, IView view)
    {
        base.OnInsert(index, view);
        UpdateZIndex();
    }

    protected override void OnRemove(int index, IView view)
    {
        base.OnRemove(index, view);
        UpdateZIndex();
    }

    protected override void OnUpdate(int index, IView view, IView oldView)
    {
        base.OnUpdate(index, view, oldView);
        UpdateZIndex();
    }

    private void UpdateZIndex()
    {
        var count = Count;
        for (var i = 0; i < count; i++)
        {
            if (this[i] is VisualElement element)
            {
                element.ZIndex = ReverseZIndex ? count - 1 - i : i;
            }
        }
    }

    //--------------------------------------------------------------------------------
    // Layout manager
    //--------------------------------------------------------------------------------

    private sealed class OverlapLayoutManager : ILayoutManager
    {
        private readonly OverlapPanel layout;

        public OverlapLayoutManager(OverlapPanel layout)
        {
            this.layout = layout;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            var padding = layout.Padding;
            var maxWidth = 0d;
            var maxHeight = 0d;
            var count = 0;
            foreach (var child in layout)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var size = child.Measure(Double.PositiveInfinity, Double.PositiveInfinity);
                maxWidth = Math.Max(maxWidth, size.Width);
                maxHeight = Math.Max(maxHeight, size.Height);
                count++;
            }

            var shift = count > 0 ? count - 1 : 0;
            return new Size(
                maxWidth + (shift * Math.Abs(layout.OffsetX)) + padding.HorizontalThickness,
                maxHeight + (shift * Math.Abs(layout.OffsetY)) + padding.VerticalThickness);
        }

        public Size ArrangeChildren(Rect bounds)
        {
            var padding = layout.Padding;
            var offsetX = layout.OffsetX;
            var offsetY = layout.OffsetY;

            var count = 0;
            foreach (var child in layout)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    count++;
                }
            }

            var shift = count > 0 ? count - 1 : 0;
            var startX = bounds.X + padding.Left + (offsetX < 0 ? shift * -offsetX : 0d);
            var startY = bounds.Y + padding.Top + (offsetY < 0 ? shift * -offsetY : 0d);

            var index = 0;
            foreach (var child in layout)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var size = child.DesiredSize;
                var x = startX + (index * offsetX);
                var y = startY + (index * offsetY);
                child.Arrange(new Rect(x, y, size.Width, size.Height));
                index++;
            }

            return bounds.Size;
        }
    }
}
