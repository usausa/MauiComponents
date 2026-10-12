namespace MauiComponents.Controls;

using Microsoft.Maui.Layouts;

public sealed class VariableSizeWrapPanel : Layout
{
    private const double UnboundedColumnWidth = 300;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(VariableSizeWrapPanel),
        2,
        propertyChanged: static (bindable, _, _) => ((VariableSizeWrapPanel)bindable).InvalidateMeasure());

    public static readonly BindableProperty RowHeightProperty = BindableProperty.Create(
        nameof(RowHeight),
        typeof(double),
        typeof(VariableSizeWrapPanel),
        Double.NaN,
        propertyChanged: static (bindable, _, _) => ((VariableSizeWrapPanel)bindable).InvalidateMeasure());

    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(VariableSizeWrapPanel),
        8d,
        propertyChanged: static (bindable, _, _) => ((VariableSizeWrapPanel)bindable).InvalidateMeasure());

    public static readonly BindableProperty ColumnSpanProperty = BindableProperty.CreateAttached(
        "ColumnSpan",
        typeof(int),
        typeof(VariableSizeWrapPanel),
        1,
        propertyChanged: OnSpanChanged);

    public static readonly BindableProperty RowSpanProperty = BindableProperty.CreateAttached(
        "RowSpan",
        typeof(int),
        typeof(VariableSizeWrapPanel),
        1,
        propertyChanged: OnSpanChanged);

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public double RowHeight
    {
        get => (double)GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Attached property
    //--------------------------------------------------------------------------------

    public static int GetColumnSpan(BindableObject bindable) => (int)bindable.GetValue(ColumnSpanProperty);

    public static void SetColumnSpan(BindableObject bindable, int value) => bindable.SetValue(ColumnSpanProperty, value);

    public static int GetRowSpan(BindableObject bindable) => (int)bindable.GetValue(RowSpanProperty);

    public static void SetRowSpan(BindableObject bindable, int value) => bindable.SetValue(RowSpanProperty, value);

    private static void OnSpanChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Element { Parent: VariableSizeWrapPanel panel })
        {
            panel.InvalidateMeasure();
        }
    }

    //--------------------------------------------------------------------------------
    // Layout
    //--------------------------------------------------------------------------------

    protected override ILayoutManager CreateLayoutManager() => new VariableSizeWrapLayoutManager(this);

    private readonly record struct Placement(IView View, int Row, int Column, int ColumnSpan, int RowSpan);

    private sealed class VariableSizeWrapLayoutManager : ILayoutManager
    {
        private readonly VariableSizeWrapPanel layout;

        public VariableSizeWrapLayoutManager(VariableSizeWrapPanel layout)
        {
            this.layout = layout;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            var columns = Math.Max(1, layout.Columns);
            var spacing = layout.Spacing;
            var padding = layout.Padding;
            var rowHeight = layout.RowHeight;
            var fixedRow = Double.IsFinite(rowHeight) && (rowHeight > 0d);

            var contentWidth = Double.IsFinite(widthConstraint)
                ? widthConstraint - padding.HorizontalThickness
                : UnboundedColumnWidth * columns;
            var cellWidth = CellSize(contentWidth, columns, spacing);

            var placements = Place(columns, out var rowCount);
            var sizes = new Size[placements.Count];
            for (var i = 0; i < placements.Count; i++)
            {
                var placement = placements[i];
                var width = Extent(cellWidth, placement.ColumnSpan, spacing);
                var height = fixedRow ? Extent(rowHeight, placement.RowSpan, spacing) : Double.PositiveInfinity;
                sizes[i] = placement.View.Measure(width, height);
            }

            var rowHeights = ComputeRowHeights(placements, sizes, rowCount, fixedRow ? rowHeight : Double.NaN, spacing);
            var contentHeight = rowHeights.Sum() + (Math.Max(0, rowCount - 1) * spacing);

            return new Size(
                contentWidth + padding.HorizontalThickness,
                contentHeight + padding.VerticalThickness);
        }

        public Size ArrangeChildren(Rect bounds)
        {
            var columns = Math.Max(1, layout.Columns);
            var spacing = layout.Spacing;
            var padding = layout.Padding;
            var rowHeight = layout.RowHeight;
            var fixedRow = Double.IsFinite(rowHeight) && (rowHeight > 0d);

            var contentWidth = bounds.Width - padding.HorizontalThickness;
            var cellWidth = CellSize(contentWidth, columns, spacing);

            var placements = Place(columns, out var rowCount);
            var sizes = new Size[placements.Count];
            for (var i = 0; i < placements.Count; i++)
            {
                sizes[i] = placements[i].View.DesiredSize;
            }

            var rowHeights = ComputeRowHeights(placements, sizes, rowCount, fixedRow ? rowHeight : Double.NaN, spacing);
            var rowOffsets = new double[rowCount];
            for (var row = 1; row < rowCount; row++)
            {
                rowOffsets[row] = rowOffsets[row - 1] + rowHeights[row - 1] + spacing;
            }

            foreach (var placement in placements)
            {
                var x = bounds.X + padding.Left + (placement.Column * (cellWidth + spacing));
                var y = bounds.Y + padding.Top + rowOffsets[placement.Row];
                var width = Extent(cellWidth, placement.ColumnSpan, spacing);
                var height = 0d;
                for (var row = placement.Row; row < placement.Row + placement.RowSpan; row++)
                {
                    height += rowHeights[row];
                }

                height += (placement.RowSpan - 1) * spacing;
                placement.View.Arrange(new Rect(x, y, width, height));
            }

            return bounds.Size;
        }

        private static double CellSize(double contentWidth, int columns, double spacing) =>
            Math.Max(0d, (contentWidth - ((columns - 1) * spacing)) / columns);

        private static double Extent(double cell, int span, double spacing) =>
            (cell * span) + ((span - 1) * spacing);

        private List<Placement> Place(int columns, out int rowCount)
        {
            var occupied = new List<bool[]>();
            var placements = new List<Placement>();
            foreach (var child in layout)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var bindable = child as BindableObject;
                var columnSpan = bindable is null ? 1 : Math.Clamp(GetColumnSpan(bindable), 1, columns);
                var rowSpan = bindable is null ? 1 : Math.Max(1, GetRowSpan(bindable));

                var (row, column) = FindSlot(occupied, columns, columnSpan, rowSpan);
                Mark(occupied, columns, row, column, columnSpan, rowSpan);
                placements.Add(new Placement(child, row, column, columnSpan, rowSpan));
            }

            rowCount = occupied.Count;
            return placements;
        }

        private static (int Row, int Column) FindSlot(List<bool[]> occupied, int columns, int columnSpan, int rowSpan)
        {
            for (var row = 0; ; row++)
            {
                for (var column = 0; column <= columns - columnSpan; column++)
                {
                    if (IsFree(occupied, row, column, columnSpan, rowSpan))
                    {
                        return (row, column);
                    }
                }
            }
        }

        private static bool IsFree(List<bool[]> occupied, int row, int column, int columnSpan, int rowSpan)
        {
            for (var r = row; r < row + rowSpan; r++)
            {
                if (r >= occupied.Count)
                {
                    return true;
                }

                var cells = occupied[r];
                for (var c = column; c < column + columnSpan; c++)
                {
                    if (cells[c])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void Mark(List<bool[]> occupied, int columns, int row, int column, int columnSpan, int rowSpan)
        {
            while (occupied.Count < row + rowSpan)
            {
                occupied.Add(new bool[columns]);
            }

            for (var r = row; r < row + rowSpan; r++)
            {
                var cells = occupied[r];
                for (var c = column; c < column + columnSpan; c++)
                {
                    cells[c] = true;
                }
            }
        }

        private static double[] ComputeRowHeights(List<Placement> placements, Size[] sizes, int rowCount, double fixedRowHeight, double spacing)
        {
            var rowHeights = new double[rowCount];
            if (Double.IsFinite(fixedRowHeight))
            {
                Array.Fill(rowHeights, fixedRowHeight);
                return rowHeights;
            }

            for (var i = 0; i < placements.Count; i++)
            {
                var placement = placements[i];
                if (placement.RowSpan == 1)
                {
                    rowHeights[placement.Row] = Math.Max(rowHeights[placement.Row], sizes[i].Height);
                }
            }

            for (var i = 0; i < placements.Count; i++)
            {
                var placement = placements[i];
                if (placement.RowSpan == 1)
                {
                    continue;
                }

                var current = (placement.RowSpan - 1) * spacing;
                for (var row = placement.Row; row < placement.Row + placement.RowSpan; row++)
                {
                    current += rowHeights[row];
                }

                var shortage = sizes[i].Height - current;
                if (shortage > 0d)
                {
                    rowHeights[placement.Row + placement.RowSpan - 1] += shortage;
                }
            }

            return rowHeights;
        }
    }
}
