namespace MauiComponents;

public interface IPopupInitialize<in T>
{
    void Initialize(T parameter);
}

public interface IPopupInitializeAsync<in T>
{
    ValueTask Initialize(T parameter);
}

public interface IPopupFactory
{
    ContentView Create(Type type);
}

public interface IPopupPlugin
{
    void Extend(ContentView view);
}

public interface IPopupNavigator
{
    ValueTask PopupAsync(object id);

    ValueTask PopupAsync<TParameter>(object id, TParameter parameter);

    ValueTask<TResult> PopupAsync<TResult>(object id);

    ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter);

    ValueTask CloseAsync();

    ValueTask CloseAsync<TResult>(TResult result);
}
