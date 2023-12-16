// ReSharper disable CheckNamespace
#pragma warning disable IDE0130
namespace MauiComponentsExample;

using Android.App;
using Android.Runtime;

[Application]
public sealed class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
