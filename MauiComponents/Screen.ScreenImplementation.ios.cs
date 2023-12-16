namespace MauiComponents;

// TODO
public sealed partial class ScreenImplementation
{
#pragma warning disable CA1822
    private partial void PlatformDispose()
    {
    }
#pragma warning restore CA1822

    public partial void SetOrientation(DisplayOrientation orientation) => throw new NotSupportedException();

    public partial void EnableDetectScreenState(bool value) => throw new NotSupportedException();
}
