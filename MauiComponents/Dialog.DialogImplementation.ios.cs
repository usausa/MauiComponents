namespace MauiComponents;

// TODO
public sealed partial class DialogImplementation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial ValueTask InformationAsync(string message, string? title, string ok) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial ValueTask<bool> ConfirmAsync(string message, string? title, string ok, string cancel, bool defaultPositive) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial ValueTask<Confirm3Result> Confirm3Async(string message, string? title, string ok, string cancel, string neutral, bool defaultPositive) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial ValueTask<int> SelectAsync(string[] items, int selected, string? title, string? cancel) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial ValueTask<PromptResult> PromptAsync(string? defaultValue, string? message, string? title, string ok, string cancel, string? placeHolder, PromptParameter? parameter) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial IDisposable Indicator() => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial void Snackbar(string message, int duration, Microsoft.Maui.Graphics.Color? color, Microsoft.Maui.Graphics.Color? textColor) => throw new NotSupportedException();
}
