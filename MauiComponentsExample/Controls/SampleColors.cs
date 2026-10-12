namespace MauiComponentsExample.Controls;

using System.Collections.ObjectModel;

public static class SampleColors
{
    public static ReadOnlyCollection<string> Names { get; } = new(["Default", "Gray", "Blue", "Teal", "Green", "Orange", "Red", "Purple", "Black", "White"]);

    public static ReadOnlyCollection<Color> Values { get; } = new(
    [
        Colors.Transparent,
        Color.FromArgb("#9E9E9E"),
        Color.FromArgb("#2196F3"),
        Color.FromArgb("#009688"),
        Color.FromArgb("#4CAF50"),
        Color.FromArgb("#FF9800"),
        Color.FromArgb("#F44336"),
        Color.FromArgb("#9C27B0"),
        Colors.Black,
        Colors.White
    ]);
}
