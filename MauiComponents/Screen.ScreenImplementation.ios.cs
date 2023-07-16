namespace MauiComponents;

// TODO
public sealed partial class ScreenImplementation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    private partial void PlatformDispose()
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial void SetOrientation(DisplayOrientation orientation) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    public partial void EnableDetectScreenState(bool value) => throw new NotSupportedException();
}
