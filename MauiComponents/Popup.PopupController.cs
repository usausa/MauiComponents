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

    public Task CloseAsync(object? value) => popup.CloseAsync(value);
}

public sealed class PopupController : IPopupController
{
    private IPopupControllerHandler? handler;

    IPopupControllerHandler? IPopupController.Handler
    {
        get => handler;
        set => handler = value;
    }

    public void Close() => handler?.Close(null);

    public Task CloseAsync()
    {
        var h = handler;
        return h is not null ? h.CloseAsync(null) : Task.CompletedTask;
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

    public Task CloseAsync(T value = default!)
    {
        var h = handler;
        return h is not null ? h.CloseAsync(value) : Task.CompletedTask;
    }
}
