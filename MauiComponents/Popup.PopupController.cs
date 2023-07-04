namespace MauiComponents;

using CommunityToolkit.Maui.Views;

internal sealed class PopupControllerHandler : IPopupControllerHandler
{
    private readonly Popup popup;

    public PopupControllerHandler(Popup popup)
    {
        this.popup = popup;
    }

    public void Close(object? value) => popup.Close(value);
}

public sealed class PopupController : IPopupController
{
    private IPopupControllerHandler? handler;

    IPopupControllerHandler? IPopupController.Handler
    {
        get => handler;
        set => handler = value;
    }

    public void Close()
    {
        handler?.Close(null);
    }
}

public sealed class PopupController<T> : IPopupController
{
    private IPopupControllerHandler? handler;

    IPopupControllerHandler? IPopupController.Handler
    {
        get => handler;
        set => handler = value;
    }

    public void Close(T value = default!)
    {
        handler?.Close(value);
    }
}
