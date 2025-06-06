namespace MauiComponents;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;

using Microsoft.Maui.Platform;

public sealed partial class DialogImplementation
{
    public async partial ValueTask InformationAsync(string message, string? title, string ok)
    {
        using var dialog = new InformationDialog(ActivityResolver.CurrentActivity, Config);
        await dialog.ShowAsync(message, title, ok).ConfigureAwait(true);
    }

    public async partial ValueTask<bool> ConfirmAsync(string message, string? title, string ok, string cancel, bool defaultPositive)
    {
        using var dialog = new ConfirmDialog(ActivityResolver.CurrentActivity, Config);
        return await dialog.ShowAsync(message, title, ok, cancel, defaultPositive).ConfigureAwait(true);
    }

    public async partial ValueTask<Confirm3Result> Confirm3Async(string message, string? title, string ok, string cancel, string neutral, bool defaultPositive)
    {
        using var dialog = new Confirm3Dialog(ActivityResolver.CurrentActivity, Config);
        return await dialog.ShowAsync(message, title, ok, cancel, neutral, defaultPositive).ConfigureAwait(true);
    }

    public async partial ValueTask<int> SelectAsync(string[] items, int selected, string? title, string? cancel)
    {
        using var dialog = new SelectDialog(ActivityResolver.CurrentActivity, Config);
        return await dialog.ShowAsync(items, selected, title, cancel).ConfigureAwait(true);
    }

    public async partial ValueTask<PromptResult> PromptAsync(string? defaultValue, string? message, string? title, string ok, string cancel, string? placeHolder, PromptParameter? parameter)
    {
        using var dialog = new PromptDialog(ActivityResolver.CurrentActivity, Config);
        return await dialog.ShowAsync(defaultValue, message, title, ok, cancel, placeHolder, parameter ?? PromptParameter.Default).ConfigureAwait(true);
    }

    public partial IDisposable Indicator()
    {
        var activity = ActivityResolver.CurrentActivity;

        var size = (int)(deviceDisplay.MainDisplayInfo.Density * Config.IndicatorSize);
#pragma warning disable CA2000
        var progress = new ProgressBar(activity)
        {
            Indeterminate = true,
            LayoutParameters = new FrameLayout.LayoutParams(size, size)
            {
                Gravity = GravityFlags.Center
            }
        };
#pragma warning restore CA2000

#pragma warning disable CA1416
#pragma warning disable CA2000
        progress.IndeterminateDrawable!.SetColorFilter(new BlendModeColorFilter(Config.IndicatorColor.ToPlatform(), BlendMode.SrcAtop!));
#pragma warning restore CA2000
#pragma warning restore CA1416

#pragma warning disable CA2000
        var layout = new FrameLayout(activity)
        {
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
        };
#pragma warning restore CA2000
        layout.AddView(progress);

#pragma warning disable CA2000
        var builder = new MaterialAlertDialogBuilder(activity);
#pragma warning restore CA2000
        builder
            .SetView(layout)!
            .SetCancelable(false);

#pragma warning disable CA2000
        builder.SetBackground(new ColorDrawable(Color.Transparent));
#pragma warning restore CA2000

        var dialog = builder.Show()!;

        return new DialogDismiss(dialog);
    }

#pragma warning disable CA1822
    public partial void Snackbar(string message, int duration, Microsoft.Maui.Graphics.Color? color, Microsoft.Maui.Graphics.Color? textColor)
    {
        var activity = ActivityResolver.CurrentActivity;
        var view = activity.Window!.DecorView.RootView!;

        var snackBar = Google.Android.Material.Snackbar.Snackbar.Make(activity, view, message, BaseTransientBottomBar.LengthShort);
        snackBar.SetDuration(duration);

        if (color is not null)
        {
            snackBar.SetBackgroundTint(color.ToPlatform());
        }

        if (textColor is not null)
        {
            snackBar.SetTextColor(textColor.ToPlatform());
        }

        snackBar.Show();
    }
#pragma warning restore CA1822

    private sealed class DialogDismiss : IDisposable
    {
        private readonly AndroidX.AppCompat.App.AlertDialog alertDialog;

        public DialogDismiss(AndroidX.AppCompat.App.AlertDialog alertDialog)
        {
            this.alertDialog = alertDialog;
        }

        public void Dispose() => alertDialog.Dismiss();
    }

    private sealed class InformationDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource result = new();

        private readonly Activity activity;

        private readonly DialogConfig config;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public InformationDialog(Activity activity, DialogConfig config)
        {
            this.activity = activity;
            this.config = config;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        public Task ShowAsync(string message, string? title, string ok)
        {
#pragma warning disable CA2000
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetOnKeyListener(this)!
                .SetCancelable(false)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult())
                .Create();
#pragma warning restore CA2000

            alertDialog.Show();

            if (config.EnableDialogButtonFocus)
            {
                var button = alertDialog.GetButton((int)DialogButtonType.Positive)!;
                button.Focusable = true;
                button.FocusableInTouchMode = true;
                button.RequestFocus();
            }

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Up) && config.DismissKeys.Contains(e.KeyCode))
            {
                alertDialog.Dismiss();
                result.TrySetResult();
                return true;
            }

            return false;
        }
    }

    private sealed class ConfirmDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<bool> result = new();

        private readonly Activity activity;

        private readonly DialogConfig config;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public ConfirmDialog(Activity activity, DialogConfig config)
        {
            this.activity = activity;
            this.config = config;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        public Task<bool> ShowAsync(string message, string? title, string ok, string cancel, bool defaultPositive)
        {
#pragma warning disable CA2000
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(true))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(false))
                .Create();
#pragma warning restore CA2000

            alertDialog.Show();

            if (config.EnableDialogButtonFocus)
            {
                var button = alertDialog.GetButton(defaultPositive ? (int)DialogButtonType.Positive : (int)DialogButtonType.Negative)!;
                button.Focusable = true;
                button.FocusableInTouchMode = true;
                button.RequestFocus();
            }

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Up) && config.DismissKeys.Contains(e.KeyCode))
            {
                alertDialog.Dismiss();
                result.TrySetResult(false);
                return true;
            }

            return false;
        }
    }

    private sealed class Confirm3Dialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<Confirm3Result> result = new();

        private readonly Activity activity;

        private readonly DialogConfig config;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public Confirm3Dialog(Activity activity, DialogConfig config)
        {
            this.activity = activity;
            this.config = config;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        public Task<Confirm3Result> ShowAsync(string message, string? title, string ok, string cancel, string neutral, bool defaultPositive)
        {
#pragma warning disable CA2000
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(Confirm3Result.Positive))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(Confirm3Result.Negative))
                .SetNeutralButton(neutral, (_, _) => result.TrySetResult(Confirm3Result.Neutral))
                .Create();
#pragma warning restore CA2000

            alertDialog.Show();

            if (config.EnableDialogButtonFocus)
            {
                var button = alertDialog.GetButton(defaultPositive ? (int)DialogButtonType.Positive : (int)DialogButtonType.Negative)!;
                button.Focusable = true;
                button.FocusableInTouchMode = true;
                button.RequestFocus();
            }

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Up) && config.DismissKeys.Contains(e.KeyCode))
            {
                alertDialog.Dismiss();
                result.TrySetResult(Confirm3Result.Negative);
                return true;
            }

            return false;
        }
    }

    private sealed class SelectDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
    {
        private readonly TaskCompletionSource<int> result = new();

        private readonly Activity activity;

        private readonly DialogConfig config;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public SelectDialog(Activity activity, DialogConfig config)
        {
            this.activity = activity;
            this.config = config;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        public Task<int> ShowAsync(string[] items, int selected, string? title, string? cancel)
        {
#pragma warning disable CA2000
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetItems(items, (_, args) => result.TrySetResult(args.Which))
                .SetOnKeyListener(this)!
                .SetCancelable(false)!
                .SetNegativeButton(cancel!, (_, _) => result.TrySetResult(-1))
                .Create();
#pragma warning restore CA2000
            alertDialog.ListView!.Selector = new ColorDrawable(config.SelectColor.ToPlatform());

            alertDialog.Show();

            if (selected >= 0)
            {
                alertDialog.ListView.SetSelection(selected);
            }

            alertDialog.ListView?.RequestFocus();

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Up) && config.DismissKeys.Contains(e.KeyCode))
            {
                alertDialog.Dismiss();
                result.TrySetResult(-1);
                return true;
            }

            return false;
        }
    }

    private sealed class PromptDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener, TextView.IOnEditorActionListener
    {
        private readonly TaskCompletionSource<PromptResult> result = new();

        private readonly Activity activity;

        private readonly DialogConfig config;

        private AndroidX.AppCompat.App.AlertDialog alertDialog = default!;

        public PromptDialog(Activity activity, DialogConfig config)
        {
            this.activity = activity;
            this.config = config;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                alertDialog.Dispose();
            }

            base.Dispose(disposing);
        }

        public Task<PromptResult> ShowAsync(string? defaultValue, string? message, string? title, string ok, string cancel, string? placeHolder, PromptParameter parameter)
        {
#pragma warning disable CA2000
            var input = new EditText(activity)
            {
                Hint = placeHolder,
                Text = defaultValue
            };
#pragma warning restore CA2000

            switch (parameter.PromptType)
            {
                case PromptType.Default:
                    input.InputType = InputTypes.ClassText;
                    break;
                case PromptType.Number:
                    input.InputType = InputTypes.ClassNumber;
                    if (parameter.Sign)
                    {
                        input.InputType |= InputTypes.NumberFlagSigned;
                    }
                    break;
                case PromptType.Decimal:
                    input.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal;
                    if (parameter.Sign)
                    {
                        input.InputType |= InputTypes.NumberFlagSigned;
                    }
                    break;
            }

            if (parameter.MaxLength > 0)
            {
                var filters = input.GetFilters()?.ToList() ?? [];
                filters.Add(new InputFilterLengthFilter(parameter.MaxLength));
                input.SetFilters([.. filters]);
            }

            if (config.EnablePromptEnterAction)
            {
                input.SetOnEditorActionListener(this);
            }

            if (config.EnablePromptSelectAll)
            {
                input.SetSelectAllOnFocus(true);
            }

#pragma warning disable CA2000
            alertDialog = new MaterialAlertDialogBuilder(activity)
                .SetTitle(title)!
                .SetMessage(message)!
                .SetView(input)!
                .SetCancelable(false)!
                .SetOnKeyListener(this)!
                .SetPositiveButton(ok, (_, _) => result.TrySetResult(new PromptResult(true, input.Text!)))
                .SetNegativeButton(cancel, (_, _) => result.TrySetResult(PromptResult.Cancel))
                .Create();
#pragma warning restore CA2000

            alertDialog.Window!.SetSoftInputMode(SoftInput.StateVisible);

            alertDialog.Show();

            input.RequestFocus();

            return result.Task;
        }

        public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Up) && config.DismissKeys.Contains(e.KeyCode) && !config.IgnorePromptDismissKeys.Contains(e.KeyCode))
            {
                alertDialog.Dismiss();
                result.TrySetResult(PromptResult.Cancel);
                return true;
            }

            return false;
        }

        public bool OnEditorAction(TextView? v, ImeAction actionId, KeyEvent? e)
        {
            if ((e is not null) && (e.Action == KeyEventActions.Down) && e.KeyCode == Keycode.Enter)
            {
                alertDialog.Dismiss();
                result.TrySetResult(new PromptResult(true, v?.Text!));
                return true;
            }

            return false;
        }
    }
}
