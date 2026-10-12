namespace MauiComponents.Controls;

public sealed class TreeNode
{
    public string Text { get; }

    public IReadOnlyList<TreeNode> Children { get; }

    public bool HasChildren => Children.Count > 0;

    public bool IsExpanded { get; set; }

    public TreeNode(string text, params TreeNode[] children)
    {
        Text = text;
        Children = children;
    }
}
