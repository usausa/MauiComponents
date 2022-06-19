namespace MauiComponents;

public static class Dialog
{
    private static DialogImplementation? current;

    public static IDialog Current => current ??= new DialogImplementation();

    public static ValueTask Information(string message, string? title = null, string ok = "OK") =>
        Current.Information(message, title, ok);

    public static ValueTask<bool> Confirm(string message, bool defaultPositive = false, string? title = null, string ok = "OK", string cancel = "Cancel") =>
        Current.Confirm(message, defaultPositive, title, ok);

    public static ValueTask<int> Select(string[] items, int selected = -1, string? title = null) =>
        Current.Select(items, selected, title);
}

internal partial class DialogImplementation : IDialog
{
    public partial ValueTask Information(string message, string? title, string ok);

    public partial ValueTask<bool> Confirm(string message, bool defaultPositive, string? title, string ok, string cancel);

    public partial ValueTask<int> Select(string[] items, int selected, string? title);
}
