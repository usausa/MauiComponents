namespace MauiComponents;

public enum Confirm3Result
{
    Positive,
    Negative,
    Neutral
}

#pragma warning disable CA1720
public enum PromptType
{
    Default,
    Number,
    Decimal
}
#pragma warning restore CA1720

public sealed class PromptParameter
{
    internal static PromptParameter Default { get; } = new();

    public PromptType PromptType { get; set; }

    public int MaxLength { get; set; }

    public bool Sign { get; set; } = true;
}

public sealed class PromptResult
{
    public static PromptResult Cancel { get; } = new(false, string.Empty);

    public bool Accepted { get; }

    public string Text { get; }

    public PromptResult(bool accepted, string text)
    {
        Accepted = accepted;
        Text = text;
    }
}

public interface ILoading : IDisposable
{
    void Update(string text);
}

public interface IProgress : IDisposable
{
    void Update(double value);
}

public interface IDialog
{
    ValueTask InformationAsync(string message, string? title = null, string ok = "OK");

    ValueTask<bool> ConfirmAsync(string message, string? title = null, string ok = "OK", string cancel = "Cancel", bool defaultPositive = false);

    ValueTask<Confirm3Result> Confirm3Async(string message, string? title = null, string ok = "Yes", string cancel = "No", string neutral = "Maybe", bool defaultPositive = false);

    ValueTask<int> SelectAsync(string[] items, int selected = -1, string? title = null, string? cancel = null);

    ValueTask<PromptResult> PromptAsync(string? defaultValue = null, string? message = null, string? title = null, string ok = "OK", string cancel = "Cancel", string? placeHolder = null, PromptParameter? parameter = null);

    IDisposable Indicator();

    IDisposable Lock();

    ILoading Loading(string text = "");

    IProgress Progress();

    void Snackbar(string message, int duration = 1000, Color? color = null, Color? textColor = null);

    ValueTask Toast(string text, bool longDuration = false, double textSize = 14);
}

public static class DialogExtensions
{
    public static async ValueTask<T?> SelectAsync<T>(this IDialog dialog, IList<T> items, Func<T, string> formatter, int selected = -1, string? title = null, string? cancel = null)
    {
        var index = await dialog.SelectAsync(items.Select(formatter).ToArray(), selected, title, cancel).ConfigureAwait(false);
        return index >= 0 ? items[index] : default;
    }
}
