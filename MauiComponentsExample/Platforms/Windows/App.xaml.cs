// ReSharper disable once CheckNamespace
namespace MauiComponentsExample.WinUI
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
