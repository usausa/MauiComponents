namespace MauiComponentsExample.Controls;

using System.Windows.Input;

using MauiComponents.Controls;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class TreeViewPageViewModel : ExtendViewModelBase
{
    public IReadOnlyList<string> IconSets { get; } = ["▾ ▸", "▼ ►", "− +"];

    [ObservableProperty]
    public partial IReadOnlyList<TreeNode> Nodes { get; set; } =
    [
        new(
            "MauiComponentsExample",
            new(
                "Dialogs",
                new("SampleDialog.xaml"),
                new("SampleDialogViewModel.cs")),
            new(
                "Platforms",
                new TreeNode(
                    "Android",
                    new("AndroidManifest.xml"),
                    new("MainActivity.cs"),
                    new("MainApplication.cs"))),
            new(
                "Resources",
                new(
                    "Fonts",
                    new("OpenSans-Regular.ttf"),
                    new("OpenSans-Semibold.ttf")),
                new(
                    "Images",
                    new TreeNode("dotnet_bot.png")),
                new(
                    "Styles",
                    new("Colors.xaml"),
                    new("Styles.xaml"))),
            new("App.xaml"),
            new("MainPage.xaml"),
            new("MauiProgram.cs"))
    ];

    [ObservableProperty]
    public partial TreeNode? SelectedNode { get; set; }

    [ObservableProperty]
    public partial double IndentSize { get; set; } = 20;

    [ObservableProperty]
    public partial double RowSpacing { get; set; } = 2;

    [ObservableProperty]
    public partial double RowPaddingHorizontal { get; set; } = 4;

    [ObservableProperty]
    public partial double RowPaddingVertical { get; set; } = 6;

    [ObservableProperty]
    public partial Thickness RowPadding { get; set; } = new(4, 6);

    [ObservableProperty]
    public partial double IconSpacing { get; set; } = 4;

    [ObservableProperty]
    public partial int IconSetIndex { get; set; }

    [ObservableProperty]
    public partial string ExpandedIcon { get; set; } = "▾";

    [ObservableProperty]
    public partial string CollapsedIcon { get; set; } = "▸";

    [ObservableProperty]
    public partial int IconColorIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedBackgroundIndex { get; set; }

    public ICommand ExpandAllCommand { get; }

    public ICommand CollapseAllCommand { get; }

    public TreeViewPageViewModel()
    {
        Nodes[0].IsExpanded = true;

        ExpandAllCommand = MakeDelegateCommand(() => ExpandAll(true));
        CollapseAllCommand = MakeDelegateCommand(() => ExpandAll(false));

        SubscribeIconSetIndex(index =>
        {
            var icons = IconSets[Math.Max(index, 0)].Split(' ');
            ExpandedIcon = icons[0];
            CollapsedIcon = icons[1];
        });
        SubscribeRowPaddingHorizontal(_ => UpdateRowPadding());
        SubscribeRowPaddingVertical(_ => UpdateRowPadding());
    }

    private void UpdateRowPadding()
    {
        RowPadding = new Thickness(RowPaddingHorizontal, RowPaddingVertical);
    }

    private void ExpandAll(bool expanded)
    {
        foreach (var node in Nodes)
        {
            SetExpanded(node, expanded);
        }

        // The tree is rebuilt when ItemsSource is replaced
        Nodes = [.. Nodes];
    }

    private static void SetExpanded(TreeNode node, bool expanded)
    {
        node.IsExpanded = expanded;
        foreach (var child in node.Children)
        {
            SetExpanded(child, expanded);
        }
    }
}
