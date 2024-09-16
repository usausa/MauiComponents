namespace MauiComponents;

using CommunityToolkit.Maui.Views;

public sealed class PopupNavigator : IPopupNavigator
{
    private readonly IPopupFactory popupFactory;

    private readonly IPopupPlugin[] plugins;

    private readonly Dictionary<object, Type> popupTypes;

    public PopupNavigator(IPopupFactory popupFactory, IEnumerable<IPopupPlugin> plugins, PopupNavigatorConfig config)
    {
        this.popupFactory = popupFactory;
        this.plugins = plugins.ToArray();
        popupTypes = new Dictionary<object, Type>(config.PopupTypes);
    }

    private Popup CreatePopup(Type type)
    {
        var popup = popupFactory.Create(type);

        foreach (var plugin in plugins)
        {
            plugin.Extend(popup);
        }

        return popup;
    }

    public async ValueTask<TResult> PopupAsync<TResult>(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = CreatePopup(type);

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }

        (popup as IDisposable)?.Dispose();
        (popup.BindingContext as IDisposable)?.Dispose();
        popup.BindingContext = null;

        return result is null ? default! : (TResult)result;
    }

    public async ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = CreatePopup(type);

        if (popup.BindingContext is IPopupInitialize<TParameter> initialize)
        {
            initialize.Initialize(parameter);
        }

        if (popup.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
        {
            await initializeAsync.Initialize(parameter).ConfigureAwait(true);
        }

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }

        (popup as IDisposable)?.Dispose();
        (popup.BindingContext as IDisposable)?.Dispose();
        popup.BindingContext = null;

        return result is null ? default! : (TResult)result;
    }

    public async ValueTask<object?> PopupAsync(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = CreatePopup(type);

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }
        (popup as IDisposable)?.Dispose();
        (popup.BindingContext as IDisposable)?.Dispose();
        popup.BindingContext = null;

        return result;
    }

    public async ValueTask<object?> PopupAsync<TParameter>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = CreatePopup(type);

        if (popup.BindingContext is IPopupInitialize<TParameter> initialize)
        {
            initialize.Initialize(parameter);
        }

        if (popup.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
        {
            await initializeAsync.Initialize(parameter).ConfigureAwait(true);
        }

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }

        (popup as IDisposable)?.Dispose();
        (popup.BindingContext as IDisposable)?.Dispose();
        popup.BindingContext = null;

        return result;
    }

    private static void Cleanup(IVisualTreeElement parent)
    {
        if (parent is VisualElement visualElement)
        {
            visualElement.Behaviors.Clear();
            visualElement.Triggers.Clear();
        }

        foreach (var child in parent.GetVisualChildren())
        {
            Cleanup(child);
        }
    }
}
