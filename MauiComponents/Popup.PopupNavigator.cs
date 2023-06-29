namespace MauiComponents;

using CommunityToolkit.Maui.Views;

public sealed class PopupNavigator : IPopupNavigator
{
    private readonly IPopupFactory popupFactory;

    private readonly Dictionary<object, Type> popupTypes;

    public PopupNavigator(IPopupFactory popupFactory, PopupNavigatorConfig config)
    {
        this.popupFactory = popupFactory;
        popupTypes = new Dictionary<object, Type>(config.PopupTypes);
    }

    public async ValueTask<TResult> PopupAsync<TResult>(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = popupFactory.Create(type);

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }
        (popup as IDisposable)?.Dispose();
        if (popup.BindingContext != popup.Parent.BindingContext)
        {
            (popup.BindingContext as IDisposable)?.Dispose();
        }
        popup.BindingContext = null;

        return (TResult)result!;
    }

    public async ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = popupFactory.Create(type);

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
        if (popup.BindingContext != popup.Parent.BindingContext)
        {
            (popup.BindingContext as IDisposable)?.Dispose();
        }
        popup.BindingContext = null;

        return (TResult)result!;
    }

    public async ValueTask<object?> PopupAsync(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = popupFactory.Create(type);

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(true);

        if (popup.Content is not null)
        {
            Cleanup(popup.Content);
        }
        (popup as IDisposable)?.Dispose();
        if (popup.BindingContext != popup.Parent.BindingContext)
        {
            (popup.BindingContext as IDisposable)?.Dispose();
        }
        popup.BindingContext = null;

        return result;
    }

    public async ValueTask<object?> PopupAsync<TParameter>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var popup = popupFactory.Create(type);

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
        if (popup.BindingContext != popup.Parent.BindingContext)
        {
            (popup.BindingContext as IDisposable)?.Dispose();
        }
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
