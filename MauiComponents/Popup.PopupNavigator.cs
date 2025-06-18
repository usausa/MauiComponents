namespace MauiComponents;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

public sealed class PopupNavigator : IPopupNavigator
{
    private readonly IPopupFactory popupFactory;

    private readonly IPopupPlugin[] plugins;

    private readonly Func<Type, object, PopupOptions> optionFactory;

    private readonly Dictionary<object, Type> popupTypes;

    public PopupNavigator(IPopupFactory popupFactory, IEnumerable<IPopupPlugin> plugins, PopupNavigatorConfig config)
    {
        this.popupFactory = popupFactory;
        this.plugins = plugins.ToArray();
        optionFactory = config.OptionFactory;
        popupTypes = new Dictionary<object, Type>(config.PopupTypes);
    }

    public async ValueTask PopupAsync(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        var option = optionFactory(type, id);
        await Application.Current!.Windows[0].Page!.ShowPopupAsync(view, option).ConfigureAwait(true);

        CloseView(view);
    }

    public async ValueTask PopupAsync<TParameter>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        if (view.BindingContext is IPopupInitialize<TParameter> initialize)
        {
            initialize.Initialize(parameter);
        }

        if (view.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
        {
            await initializeAsync.Initialize(parameter).ConfigureAwait(true);
        }

        var option = optionFactory(type, id);
        await Application.Current!.Windows[0].Page!.ShowPopupAsync(view, option).ConfigureAwait(true);

        CloseView(view);
    }

    public async ValueTask<TResult> PopupAsync<TResult>(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        var option = optionFactory(type, id);
        var result = await Application.Current!.Windows[0].Page!.ShowPopupAsync<TResult>(view, option).ConfigureAwait(true);

        CloseView(view);

        return result.Result ?? default!;
    }

    public async ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        if (view.BindingContext is IPopupInitialize<TParameter> initialize)
        {
            initialize.Initialize(parameter);
        }

        if (view.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
        {
            await initializeAsync.Initialize(parameter).ConfigureAwait(true);
        }

        var option = optionFactory(type, id);
        var result = await Application.Current!.Windows[0].Page!.ShowPopupAsync<TResult>(view, option).ConfigureAwait(true);

        CloseView(view);

        return result.Result ?? default!;
    }

    public async ValueTask CloseAsync()
    {
        await Application.Current!.Windows[0].Page!.ClosePopupAsync().ConfigureAwait(true);
    }

    public async ValueTask CloseAsync<TResult>(TResult result)
    {
        await Application.Current!.Windows[0].Page!.ClosePopupAsync(result).ConfigureAwait(true);
    }

    private ContentView CreateView(Type type)
    {
        var view = popupFactory.Create(type);

        foreach (var plugin in plugins)
        {
            plugin.Extend(view);
        }

        return view;
    }

    private static void CloseView(ContentView view)
    {
        view.Behaviors.Clear();
        view.Triggers.Clear();

        if (view.Content is not null)
        {
            Cleanup(view.Content);
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (view is IDisposable disposable)
        {
            disposable.Dispose();
        }

        (view.BindingContext as IDisposable)?.Dispose();
        view.BindingContext = null;
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
