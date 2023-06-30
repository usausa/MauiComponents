namespace MauiComponents;

public enum Confirm3Result
{
    Positive,
    Negative,
    Neutral
}

#pragma warning disable CA1720
public enum InputType
{
    Default,
    Email,
    Number,
    Decimal
}
#pragma warning restore CA1720

public sealed class InputResult
{
    public static InputResult Cancel { get; } = new(false, string.Empty);

    public bool Accepted { get; }

    public string Text { get; }

    public InputResult(bool accepted, string text)
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

    ValueTask<bool> ConfirmAsync(string message, bool defaultPositive = false, string? title = null, string ok = "OK", string cancel = "Cancel");

    ValueTask<Confirm3Result> Confirm3Async(string message, bool defaultPositive = false, string? title = null, string ok = "Yes", string cancel = "No", string neutral = "Maybe");

    ValueTask<int> SelectAsync(string[] items, int selected = -1, string? title = null);

    ValueTask<InputResult> InputAsync(string? defaultValue = null, string? message = null, string? title = null, string ok = "OK", string cancel = "Cancel", InputType inputType = InputType.Default, int maxLength = 0, string? placeHolder = null);

    IDisposable Indicator();

    IDisposable Lock();

    ILoading Loading(string text = "");

    IProgress Progress();

    void Snackbar(string message, int duration = 1000, Color? color = null, Color? textColor = null);
}
