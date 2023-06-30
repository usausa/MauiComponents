namespace MauiComponents;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;

using AndroidX.Core.OS;

using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;

using Microsoft.Maui.Controls.Compatibility.Platform.Android;

internal sealed partial class DialogImplementation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public async partial ValueTask InformationAsync(string message, string? title, string ok)
    {
        using var dialog = new InformationDialog(ActivityResolver.CurrentActivity, Options);
        await dialog.ShowAsync(message, title, ok).ConfigureAwait(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public async partial ValueTask<bool> ConfirmAsync(string message, bool defaultPositive, string? title, string ok, string cancel)
    {
        using var dialog = new ConfirmDialog(ActivityResolver.CurrentActivity, Options);
        return await dialog.ShowAsync(message, defaultPositive, title, ok, cancel).ConfigureAwait(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public async partial ValueTask<Confirm3Result> Confirm3Async(string message, bool defaultPositive, string? title, string ok, string cancel, string neutral)
    {
        using var dialog = new Confirm3Dialog(ActivityResolver.CurrentActivity, Options);
        return await dialog.ShowAsync(message, defaultPositive, title, ok, cancel, neutral).ConfigureAwait(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public async partial ValueTask<int> SelectAsync(string[] items, int selected, string? title)
    {
        using var dialog = new SelectDialog(ActivityResolver.CurrentActivity, Options);
        return await dialog.ShowAsync(items, selected, title).ConfigureAwait(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public async partial ValueTask<InputResult> InputAsync(string? defaultValue, string? message, string? title, string ok, string cancel, InputType inputType, int maxLength, string? placeHolder)
    {
        using var dialog = new InputDialog(ActivityResolver.CurrentActivity, Options);
        return await dialog.ShowAsync(defaultValue, message, title, ok, cancel, inputType, maxLength, placeHolder).ConfigureAwait(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
    public partial IDisposable Indicator()
    {
        var activity = ActivityResolver.CurrentActivity;

        var size = (int)(DeviceDisplay.MainDisplayInfo.Density * Options.IndicatorSize);
        var progress = new ProgressBar(activity)
        {
            Indeterminate = true,
            LayoutParameters = new FrameLayout.LayoutParams(size, size)
            {
                Gravity = GravityFlags.Center
            }
        };

        if (Options.IndicatorColor is not null)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
#pragma warning disable CA1416
                progress.IndeterminateDrawable!.SetColorFilter(new BlendModeColorFilter(Options.IndicatorColor.ToAndroid(), BlendMode.SrcAtop!));
#pragma warning restore CA1416
            }
            else
            {
#pragma warning disable CS0618
                progress.IndeterminateDrawable!.SetColorFilter(Colors.Blue.ToAndroid(), PorterDuff.Mode.SrcAtop!);
#pragma warning restore CS0618
            }
        }

        var layout = new FrameLayout(activity)
        {
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
        };
        layout.AddView(progress);

        var builder = new MaterialAlertDialogBuilder(activity);
        builder
            .SetView(layout)!
            .SetCancelable(false);

        builder.SetBackground(new ColorDrawable(Android.Graphics.Color.Transparent));

        var dialog = builder.Show()!;

        return new DialogDismiss(dialog);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial void Snackbar(string message, int duration, Microsoft.Maui.Graphics.Color? color, Microsoft.Maui.Graphics.Color? textColor)
    {
        var activity = ActivityResolver.CurrentActivity;
        var view = activity.Window!.DecorView.RootView!;

        var snackBar = Google.Android.Material.Snackbar.Snackbar.Make(activity, view, message, BaseTransientBottomBar.LengthShort);
        snackBar.SetDuration(duration);

        if (color is not null)
        {
            snackBar.SetBackgroundTint(color.ToAndroid());
        }

        if (textColor is not null)
        {
            snackBar.SetTextColor(textColor.ToAndroid());
        }

        snackBar.Show();
    }

    private sealed class DialogDismiss : IDisposable
    {
        private readonly AndroidX.AppCompat.App.AlertDialog alertDialog;

        public DialogDismiss(AndroidX.AppCompat.App.AlertDialog alertDialog)
        {
            this.alertDialog = alertDialog;
        }

        public void Dispose() => alertDialog.Dismiss();
    }

    private sealed class InformationDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<bool> result = new();

        private readonly Activity activity;

        private readonly DialogOptions options;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public InformationDialog(Activity activity, DialogOptions options)
        {
            this.activity = activity;
            this.options = options;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public Task ShowAsync(string message, string? title, string ok)
        {
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetOnKeyListener(this)!
                .SetCancelable(false)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(true))
                .Create();
            alertDialog.SetOnShowListener(this);

            alertDialog.Show();

            return result.Task;
        }

        public void OnShow(IDialogInterface? dialog)
        {
            var button = alertDialog.GetButton((int)DialogButtonType.Positive)!;
            button.RequestFocus();
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e!.Action == KeyEventActions.Up) && options.DismissKeys.Contains(e.KeyCode))
            {
                dialog!.Dismiss();
                result.TrySetResult(false);
                return true;
            }

            return false;
        }
    }

    private sealed class ConfirmDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<bool> result = new();

        private readonly Activity activity;

        private readonly DialogOptions options;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        private bool positive;

        public ConfirmDialog(Activity activity, DialogOptions options)
        {
            this.activity = activity;
            this.options = options;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public Task<bool> ShowAsync(string message, bool defaultPositive, string? title, string ok, string cancel)
        {
            positive = defaultPositive;

            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(true))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(false))
                .Create();
            alertDialog.SetOnShowListener(this);

            alertDialog.Show();

            return result.Task;
        }

        public void OnShow(IDialogInterface? dialog)
        {
            var button = alertDialog.GetButton(positive ? (int)DialogButtonType.Positive : (int)DialogButtonType.Negative)!;
            button.RequestFocus();
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e!.Action == KeyEventActions.Up) && options.DismissKeys.Contains(e.KeyCode))
            {
                dialog!.Dismiss();
                result.TrySetResult(false);
                return true;
            }

            return false;
        }
    }

    private sealed class Confirm3Dialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<Confirm3Result> result = new();

        private readonly Activity activity;

        private readonly DialogOptions options;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        private bool positive;

        public Confirm3Dialog(Activity activity, DialogOptions options)
        {
            this.activity = activity;
            this.options = options;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public Task<Confirm3Result> ShowAsync(string message, bool defaultPositive, string? title, string ok, string cancel, string neutral)
        {
            positive = defaultPositive;

            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(Confirm3Result.Positive))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(Confirm3Result.Negative))
                .SetNeutralButton(neutral, (_, _) => result.TrySetResult(Confirm3Result.Neutral))
                .Create();
            alertDialog.SetOnShowListener(this);

            alertDialog.Show();

            return result.Task;
        }

        public void OnShow(IDialogInterface? dialog)
        {
            var button = alertDialog.GetButton(positive ? (int)DialogButtonType.Positive : (int)DialogButtonType.Negative)!;
            button.RequestFocus();
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e!.Action == KeyEventActions.Up) && options.DismissKeys.Contains(e.KeyCode))
            {
                dialog!.Dismiss();
                result.TrySetResult(Confirm3Result.Negative);
                return true;
            }

            return false;
        }
    }

    private sealed class SelectDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<int> result = new();

        private readonly Activity activity;

        private readonly DialogOptions options;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public SelectDialog(Activity activity, DialogOptions options)
        {
            this.activity = activity;
            this.options = options;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public Task<int> ShowAsync(string[] items, int selected, string? title)
        {
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetItems(items, (_, args) => result.TrySetResult(args.Which))
                .SetOnKeyListener(this)!
                .SetCancelable(false)!
                .Create();
            alertDialog.SetOnShowListener(this);
            alertDialog.ListView!.Selector = new ColorDrawable(options.SelectColor.ToAndroid());

            alertDialog.Show();

            if (selected >= 0)
            {
                alertDialog.ListView.SetSelection(selected);
            }

            return result.Task;
        }

        public void OnShow(IDialogInterface? dialog)
        {
            alertDialog.ListView?.RequestFocus();
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e!.Action == KeyEventActions.Up) && options.DismissKeys.Contains(e.KeyCode))
            {
                dialog!.Dismiss();
                result.TrySetResult(-1);
                return true;
            }

            return false;
        }
    }

    private sealed class InputDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<InputResult> result = new();

        private readonly Activity activity;

        private readonly DialogOptions options;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public InputDialog(Activity activity, DialogOptions options)
        {
            this.activity = activity;
            this.options = options;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public Task<InputResult> ShowAsync(string? defaultValue, string? message, string? title, string ok, string cancel, InputType inputType, int maxLength, string? placeHolder)
        {
            var input = new EditText(activity)
            {
                Hint = placeHolder,
                Text = defaultValue
            };

            // TODO Ex
            switch (inputType)
            {
                case InputType.Default:
                    input.InputType = InputTypes.ClassText;
                    break;
                case InputType.Email:
                    input.InputType = InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
                    break;
                case InputType.Number:
                    input.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned;
                    break;
                case InputType.Decimal:
                    input.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned | InputTypes.NumberFlagDecimal;
                    var locale = LocaleListCompat.Default.Get(0);
                    input.KeyListener = new DigitsKeyListener(locale, true, true);
                    break;
            }

            if (maxLength > 0)
            {
                var filters = input.GetFilters()?.ToList() ?? new List<IInputFilter>();
                filters.Add(new InputFilterLengthFilter(maxLength));
                input.SetFilters(filters.ToArray());
            }

            // TODO show keyboard
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetView(input)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(new InputResult(true, input.Text!)))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(InputResult.Cancel))
                .Create();

            alertDialog.Show();

            input.RequestFocus();

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            // TODO enter?
            if ((e!.Action == KeyEventActions.Up) && options.DismissKeys.Contains(e.KeyCode))
            {
                dialog!.Dismiss();
                result.TrySetResult(InputResult.Cancel);
                return true;
            }

            return false;
        }
    }
}
