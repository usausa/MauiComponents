namespace MauiComponents;

using CoreGraphics;

using UIKit;

public sealed partial class DialogImplementation
{
#pragma warning disable CA1822
    public async partial ValueTask InformationAsync(string message, string? title, string ok)
    {
        var tcs = new TaskCompletionSource();
#pragma warning disable CA2000
        var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
#pragma warning restore CA2000
#pragma warning disable CA2000
        alert.AddAction(UIAlertAction.Create(ok, UIAlertActionStyle.Default, _ => tcs.TrySetResult()));
#pragma warning restore CA2000
        await PresentAsync(alert).ConfigureAwait(true);
        await tcs.Task.ConfigureAwait(true);
    }
#pragma warning restore CA1822

    public partial ValueTask<bool> ConfirmAsync(string message, string? title, string ok, string cancel, bool defaultPositive) => throw new NotSupportedException();

    public partial ValueTask<Confirm3Result> Confirm3Async(string message, string? title, string ok, string cancel, string neutral, bool defaultPositive) => throw new NotSupportedException();

#pragma warning disable CA1822
    public async partial ValueTask<int> SelectAsync(string[] items, int selected, string? title, string? cancel)
    {
        var tcs = new TaskCompletionSource<int>();
#pragma warning disable CA2000
        var alert = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);
#pragma warning restore CA2000
        for (var i = 0; i < items.Length; i++)
        {
            var index = i;
#pragma warning disable CA2000
            alert.AddAction(UIAlertAction.Create(items[i], UIAlertActionStyle.Default, _ => tcs.TrySetResult(index)));
#pragma warning restore CA2000
        }

        if (cancel is not null)
        {
#pragma warning disable CA2000
            alert.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, _ => tcs.TrySetResult(-1)));
#pragma warning restore CA2000
        }

        await PresentAsync(alert).ConfigureAwait(true);
        return await tcs.Task.ConfigureAwait(true);
    }
#pragma warning restore CA1822

#pragma warning disable CA1822
    public async partial ValueTask<PromptResult> PromptAsync(string? defaultValue, string? message, string? title, string ok, string cancel, string? placeHolder, PromptParameter? parameter)
    {
        var param = parameter ?? PromptParameter.Default;
        var tcs = new TaskCompletionSource<PromptResult>();
        UITextField? textField = null;

#pragma warning disable CA2000
        var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
#pragma warning restore CA2000
        alert.AddTextField(field =>
        {
            textField = field;
            field.Text = defaultValue;
            field.Placeholder = placeHolder;
            field.KeyboardType = param.PromptType switch
            {
                PromptType.Number => param.Sign ? UIKeyboardType.NumbersAndPunctuation : UIKeyboardType.NumberPad,
                PromptType.Decimal => UIKeyboardType.DecimalPad,
                _ => UIKeyboardType.Default
            };
            if (param.MaxLength > 0)
            {
                var maxLength = param.MaxLength;
                field.ShouldChangeCharacters = (f, range, replacement) =>
                    (f.Text?.Length ?? 0) - range.Length + replacement.Length <= maxLength;
            }

            if (!String.IsNullOrEmpty(defaultValue))
            {
                field.SelectAll(field);
            }
        });

#pragma warning disable CA2000
        alert.AddAction(UIAlertAction.Create(
            ok,
            UIAlertActionStyle.Default,
            _ => tcs.TrySetResult(new PromptResult(true, textField?.Text ?? string.Empty))));
#pragma warning restore CA2000
#pragma warning disable CA2000
        alert.AddAction(UIAlertAction.Create(
            cancel,
            UIAlertActionStyle.Cancel,
            _ => tcs.TrySetResult(PromptResult.Cancel)));
#pragma warning restore CA2000

        await PresentAsync(alert).ConfigureAwait(true);
        return await tcs.Task.ConfigureAwait(true);
    }
#pragma warning restore CA1822

    public partial IDisposable Indicator() => throw new NotSupportedException();

    public partial void Snackbar(string message, int duration, Microsoft.Maui.Graphics.Color? color, Microsoft.Maui.Graphics.Color? textColor) => throw new NotSupportedException();

    private static async Task PresentAsync(UIAlertController alert)
    {
        var vc = Platform.GetCurrentUIViewController()!;
        if (alert.PopoverPresentationController is { } popover)
        {
            popover.SourceView = vc.View!;
            popover.SourceRect = new CGRect(
                vc.View!.Bounds.GetMidX(), vc.View.Bounds.GetMidY(), 0, 0);
            popover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
        }

        await vc.PresentViewControllerAsync(alert, true).ConfigureAwait(true);
    }
}
