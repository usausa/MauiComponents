namespace MauiComponents.Controls;

public sealed class TreeView : ContentView
{
    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IReadOnlyList<TreeNode>),
        typeof(TreeView),
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty SelectedNodeProperty = BindableProperty.Create(
        nameof(SelectedNode),
        typeof(TreeNode),
        typeof(TreeView),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty IndentSizeProperty = BindableProperty.Create(
        nameof(IndentSize),
        typeof(double),
        typeof(TreeView),
        20d,
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty ExpandedIconProperty = BindableProperty.Create(
        nameof(ExpandedIcon),
        typeof(string),
        typeof(TreeView),
        "▾",
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty CollapsedIconProperty = BindableProperty.Create(
        nameof(CollapsedIcon),
        typeof(string),
        typeof(TreeView),
        "▸",
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty IconFontFamilyProperty = BindableProperty.Create(
        nameof(IconFontFamily),
        typeof(string),
        typeof(TreeView),
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(TreeView),
        Color.FromArgb("#616161"),
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty BranchIconProperty = BindableProperty.Create(
        nameof(BranchIcon),
        typeof(string),
        typeof(TreeView),
        "📁",
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty LeafIconProperty = BindableProperty.Create(
        nameof(LeafIcon),
        typeof(string),
        typeof(TreeView),
        "📄",
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TreeView),
        Color.FromArgb("#37474F"),
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public static readonly BindableProperty SelectedBackgroundProperty = BindableProperty.Create(
        nameof(SelectedBackground),
        typeof(Color),
        typeof(TreeView),
        Color.FromArgb("#BBDEFB"),
        propertyChanged: static (bindable, _, _) => ((TreeView)bindable).Rebuild());

    public IReadOnlyList<TreeNode>? ItemsSource
    {
        get => (IReadOnlyList<TreeNode>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public TreeNode? SelectedNode
    {
        get => (TreeNode?)GetValue(SelectedNodeProperty);
        set => SetValue(SelectedNodeProperty, value);
    }

    public double IndentSize
    {
        get => (double)GetValue(IndentSizeProperty);
        set => SetValue(IndentSizeProperty, value);
    }

    public string ExpandedIcon
    {
        get => (string)GetValue(ExpandedIconProperty);
        set => SetValue(ExpandedIconProperty, value);
    }

    public string CollapsedIcon
    {
        get => (string)GetValue(CollapsedIconProperty);
        set => SetValue(CollapsedIconProperty, value);
    }

    public string? IconFontFamily
    {
        get => (string?)GetValue(IconFontFamilyProperty);
        set => SetValue(IconFontFamilyProperty, value);
    }

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public string BranchIcon
    {
        get => (string)GetValue(BranchIconProperty);
        set => SetValue(BranchIconProperty, value);
    }

    public string LeafIcon
    {
        get => (string)GetValue(LeafIconProperty);
        set => SetValue(LeafIconProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color SelectedBackground
    {
        get => (Color)GetValue(SelectedBackgroundProperty);
        set => SetValue(SelectedBackgroundProperty, value);
    }

    private readonly VerticalStackLayout container;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public TreeView()
    {
        container = new VerticalStackLayout { Spacing = 2 };
        Content = container;
    }

    //--------------------------------------------------------------------------------
    // Content
    //--------------------------------------------------------------------------------

    private void Rebuild()
    {
        container.Children.Clear();

        if (ItemsSource is { } roots)
        {
            AddNodes(roots, 0);
        }
    }

    private void AddNodes(IEnumerable<TreeNode> nodes, int depth)
    {
        foreach (var node in nodes)
        {
            container.Children.Add(BuildRow(node, depth));
            if (node.HasChildren && node.IsExpanded)
            {
                AddNodes(node.Children, depth + 1);
            }
        }
    }

    private Grid BuildRow(TreeNode node, int depth)
    {
        var row = new Grid
        {
            Padding = new Thickness((depth * IndentSize) + 4, 6, 4, 6),
            ColumnSpacing = 4,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            },
            BackgroundColor = ReferenceEquals(node, SelectedNode) ? SelectedBackground : Colors.Transparent
        };

        var chevron = new Label
        {
            Text = node.HasChildren ? (node.IsExpanded ? ExpandedIcon : CollapsedIcon) : string.Empty,
            FontFamily = IconFontFamily,
            TextColor = IconColor,
            FontSize = 20,
            WidthRequest = 20,
            VerticalTextAlignment = TextAlignment.Center
        };
        row.Add(chevron, 0);

        var icon = new Label
        {
            Text = node.HasChildren ? BranchIcon : LeafIcon,
            FontSize = 14,
            VerticalTextAlignment = TextAlignment.Center
        };
        row.Add(icon, 1);

        var text = new Label
        {
            Text = node.Text,
            TextColor = TextColor,
            FontSize = 14,
            VerticalTextAlignment = TextAlignment.Center
        };
        row.Add(text, 2);

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) =>
        {
            if (node.HasChildren)
            {
                node.IsExpanded = !node.IsExpanded;
            }

            SelectedNode = node;
            Rebuild();
        };
        row.GestureRecognizers.Add(tap);

        return row;
    }
}
