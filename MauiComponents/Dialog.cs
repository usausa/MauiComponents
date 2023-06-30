namespace MauiComponents;

public enum Confirm3Result
{
    Positive,
    Negative,
    Neutral
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

    IDisposable Lock();

    ILoading Loading(string text = "");

    IProgress Progress();
}
