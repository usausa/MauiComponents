namespace MauiComponents.Controls;

using Microsoft.Maui.Layouts;

public sealed class StaggeredGrid : Layout
{
    private const double UnboundedColumnWidth = 300;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(StaggeredGrid),
        2,
        propertyChanged: static (bindable, _, _) => ((StaggeredGrid)bindable).InvalidateMeasure());

    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(StaggeredGrid),
        4d,
        propertyChanged: static (bindable, _, _) => ((StaggeredGrid)bindable).InvalidateMeasure());

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Layout
    //--------------------------------------------------------------------------------

    protected override ILayoutManager CreateLayoutManager() => new StaggeredGridLayoutManager(this);

    private sealed class StaggeredGridLayoutManager : ILayoutManager
    {
        private readonly StaggeredGrid layout;

        public StaggeredGridLayoutManager(StaggeredGrid layout)
        {
            this.layout = layout;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            var columns = Math.Max(1, layout.Columns);
            var spacing = layout.Spacing;
            var padding = layout.Padding;

            var contentWidth = Double.IsFinite(widthConstraint)
                ? widthConstraint - padding.HorizontalThickness
                : UnboundedColumnWidth * columns;
            var columnWidth = (contentWidth - ((columns - 1) * spacing)) / columns;

            var heights = new double[columns];
            foreach (var child in layout)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var size = child.Measure(columnWidth, Double.PositiveInfinity);
                var column = ShortestColumn(heights);
                heights[column] += size.Height + spacing;
            }

            var contentHeight = heights.Max();
            if (contentHeight > 0d)
            {
                contentHeight -= spacing;
            }

            return new Size(
                contentWidth + padding.HorizontalThickness,
                contentHeight + padding.VerticalThickness);
        }

        public Size ArrangeChildren(Rect bounds)
        {
            var columns = Math.Max(1, layout.Columns);
            var spacing = layout.Spacing;
            var padding = layout.Padding;

            var contentWidth = bounds.Width - padding.HorizontalThickness;
            var columnWidth = (contentWidth - ((columns - 1) * spacing)) / columns;

            var heights = new double[columns];
            foreach (var child in layout)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var column = ShortestColumn(heights);
                var x = bounds.X + padding.Left + (column * (columnWidth + spacing));
                var y = bounds.Y + padding.Top + heights[column];
                var height = child.DesiredSize.Height;
                child.Arrange(new Rect(x, y, columnWidth, height));
                heights[column] += height + spacing;
            }

            return bounds.Size;
        }

        private static int ShortestColumn(double[] heights)
        {
            var column = 0;
            for (var i = 1; i < heights.Length; i++)
            {
                if (heights[i] < heights[column])
                {
                    column = i;
                }
            }

            return column;
        }
    }
}
