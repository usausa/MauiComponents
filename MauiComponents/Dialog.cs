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
        var index = await dialog.SelectAsync(items.Select(formatter).ToArray(), selected, title, cancel).ConfigureAwait(true);
        return index >= 0 ? items[index] : default;
    }
}

// TODO
#pragma warning disable SA1005
//public sealed class DialogService : IDialogService
//{
//    public async Task<string?> DisplayActionSheet(string? title, string? cancelButton, string? destroyButton, params string[] otherButtons)
//    {
//        return await (Application.Current?.MainPage?.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons) ?? Task.FromResult(default(string)));
//    }

//    public async Task DisplayActionSheet(string? title, params IActionSheetButton[] buttons)
//    {
//        var cancelButton = buttons.FirstOrDefault(static b => b.ButtonType == ActionSheetButtonType.Cancel);
//        var destroyButton = buttons.FirstOrDefault(static b => b.ButtonType == ActionSheetButtonType.Destroy);
//        var otherButtonTexts = buttons.Where(static b => b.ButtonType == ActionSheetButtonType.Other).Select(static b => b.Text).ToArray();

//        var selectedText = await DisplayActionSheet(title, cancelButton?.Text, destroyButton?.Text, otherButtonTexts);

//        var selectedButton = buttons.FirstOrDefault(b => b.Text == selectedText);
//        if (selectedButton is not null)
//        {
//            await selectedButton.Execute();
//        }
//    }
//}

//public enum ActionSheetButtonType
//{
//    Other,
//    Cancel,
//    Destroy
//}

//public interface IActionSheetButton
//{
//    ActionSheetButtonType ButtonType { get; }

//    string Text { get; }

//    Task Execute();
//}

//internal sealed class NoParameterActionSheetButton : IActionSheetButton
//{
//    public ActionSheetButtonType ButtonType { get; }

//    public string Text { get; }

//    private readonly Action? action;

//    private readonly Func<Task>? asyncAction;

//    internal NoParameterActionSheetButton(ActionSheetButtonType buttonType, string text, Action action)
//    {
//        ButtonType = buttonType;
//        Text = text;
//        this.action = action;
//        asyncAction = null;
//    }

//    internal NoParameterActionSheetButton(ActionSheetButtonType buttonType, string text, Func<Task>? asyncAction)
//    {
//        ButtonType = buttonType;
//        Text = text;
//        action = null;
//        this.asyncAction = asyncAction;
//    }

//    public async Task Execute()
//    {
//        if (action is not null)
//        {
//            action();
//        }
//        else if (asyncAction is not null)
//        {
//            await asyncAction();
//        }
//    }
//}

//internal sealed class ParameterActionSheetButton<T> : IActionSheetButton
//{
//    public ActionSheetButtonType ButtonType { get; }

//    public string Text { get; }

//    private readonly Action<T>? action;

//    private readonly Func<T, Task>? asyncAction;

//    private readonly T parameter;

//    internal ParameterActionSheetButton(ActionSheetButtonType buttonType, string text, T parameter, Action<T> action)
//    {
//        ButtonType = buttonType;
//        Text = text;
//        this.parameter = parameter;
//        this.action = action;
//        asyncAction = null;
//    }

//    internal ParameterActionSheetButton(ActionSheetButtonType buttonType, string text, T parameter, Func<T, Task>? asyncAction)
//    {
//        ButtonType = buttonType;
//        Text = text;
//        this.parameter = parameter;
//        action = null;
//        this.asyncAction = asyncAction;
//    }

//    public async Task Execute()
//    {
//        if (action is not null)
//        {
//            action(parameter);
//        }
//        else if (asyncAction is not null)
//        {
//            await asyncAction(parameter);
//        }
//    }
//}

//public sealed class ActionSheetButton
//{
//    public static IActionSheetButton Create(string text, Action action) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Other, text, action);

//    public static IActionSheetButton Create(string text, Func<Task> asyncAction) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Other, text, asyncAction);

//    public static IActionSheetButton Create<T>(string text, T parameter, Action<T> action) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Other, text, parameter, action);

//    public static IActionSheetButton Create<T>(string text, T parameter, Func<T, Task> asyncAction) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Other, text, parameter, asyncAction);

//    public static IActionSheetButton CreateCancel(string text, Action action) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Cancel, text, action);

//    public static IActionSheetButton CreateCancel(string text, Func<Task> asyncAction) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Cancel, text, asyncAction);

//    public static IActionSheetButton CreateCancel<T>(string text, T parameter, Action<T> action) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Cancel, text, parameter, action);

//    public static IActionSheetButton CreateCancel<T>(string text, T parameter, Func<T, Task> asyncAction) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Cancel, text, parameter, asyncAction);

//    public static IActionSheetButton CreateDestroy(string text, Action action) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Destroy, text, action);

//    public static IActionSheetButton CreateDestroy(string text, Func<Task> asyncAction) =>
//        new NoParameterActionSheetButton(ActionSheetButtonType.Destroy, text, asyncAction);

//    public static IActionSheetButton CreateDestroy<T>(string text, T parameter, Action<T> action) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Destroy, text, parameter, action);

//    public static IActionSheetButton CreateDestroy<T>(string text, T parameter, Func<T, Task> asyncAction) =>
//        new ParameterActionSheetButton<T>(ActionSheetButtonType.Destroy, text, parameter, asyncAction);
//}
