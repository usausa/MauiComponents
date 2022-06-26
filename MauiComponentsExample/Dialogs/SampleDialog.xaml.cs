namespace MauiComponentsExample.Dialogs;

using MauiComponents;

[Popup(DialogId.Sample)]
public partial class SampleDialog
{
    public SampleDialog()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Close();
    }
}
