namespace MauiComponents.Controls;

public sealed partial class SideDrawer
{
    //--------------------------------------------------------------------------------
    // Platform
    //--------------------------------------------------------------------------------

    private partial Grid CreatePanel()
    {
        var grid = new Grid();
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        grid.GestureRecognizers.Add(pan);
        return grid;
    }
}
