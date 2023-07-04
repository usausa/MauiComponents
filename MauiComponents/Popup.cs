namespace MauiComponents;

using CommunityToolkit.Maui.Views;

public interface IPopupInitialize<in T>
{
    void Initialize(T parameter);
}

public interface IPopupInitializeAsync<in T>
{
    ValueTask Initialize(T parameter);
}

public interface IPopupControllerHandler
{
    void Close(object? value);
}

public interface IPopupController
{
    IPopupControllerHandler? Handler { get; set; }
}

public interface IPopupFactory
{
    Popup Create(Type type);
}

public interface IPopupNavigator
{
    ValueTask<TResult> PopupAsync<TResult>(object id);

    ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter);

    ValueTask<object?> PopupAsync(object id);

    ValueTask<object?> PopupAsync<TParameter>(object id, TParameter parameter);
}
