namespace MauiComponents;

public interface IDialog
{
    ValueTask Information(string message, string? title = null, string ok = "OK");

    ValueTask<bool> Confirm(string message, bool defaultPositive = false, string? title = null, string ok = "OK", string cancel = "Cancel");

    ValueTask<int> Select(string[] items, int selected = -1, string? title = null);
}
