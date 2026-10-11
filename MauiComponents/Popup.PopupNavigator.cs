namespace MauiComponents;

using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

public sealed class PopupNavigator : IPopupNavigator
{
    private readonly IPopupFactory popupFactory;

    private readonly IPopupPlugin[] plugins;

    private readonly Func<Type, object, PopupOptions>? optionFactory;

    private readonly Dictionary<object, Type> popupTypes;

    private readonly List<ContentView> views = [];

    private int closingDepth;

    public PopupNavigator(IPopupFactory popupFactory, IEnumerable<IPopupPlugin> plugins, PopupNavigatorConfig config)
    {
        this.popupFactory = popupFactory;
#pragma warning disable IDE0028
        this.plugins = plugins.ToArray();
#pragma warning restore IDE0028
        optionFactory = config.OptionFactory;
#pragma warning disable IDE0028
        popupTypes = new Dictionary<object, Type>(config.PopupTypes);
#pragma warning restore IDE0028
    }

    public async ValueTask PopupAsync(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        OpenView(view);
        try
        {
            var option = optionFactory?.Invoke(type, id);
            await Application.Current!.Windows[0].Page!.ShowPopupAsync(view, option).ConfigureAwait(true);
        }
        finally
        {
            await CloseViewAsync(view).ConfigureAwait(true);
        }
    }

    public async ValueTask PopupAsync<TParameter>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        OpenView(view);
        try
        {
            if (view.BindingContext is IPopupInitialize<TParameter> initialize)
            {
                initialize.Initialize(parameter);
            }

            if (view.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
            {
                await initializeAsync.Initialize(parameter).ConfigureAwait(true);
            }

            var option = optionFactory?.Invoke(type, id);
            await Application.Current!.Windows[0].Page!.ShowPopupAsync(view, option).ConfigureAwait(true);
        }
        finally
        {
            await CloseViewAsync(view).ConfigureAwait(true);
        }
    }

    public async ValueTask<TResult> PopupAsync<TResult>(object id)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        OpenView(view);
        try
        {
            var option = optionFactory?.Invoke(type, id);
            var result = await Application.Current!.Windows[0].Page!.ShowPopupAsync<TResult>(view, option).ConfigureAwait(true);
            return result.Result ?? default!;
        }
        finally
        {
            await CloseViewAsync(view).ConfigureAwait(true);
        }
    }

    public async ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter)
    {
        if (!popupTypes.TryGetValue(id, out var type))
        {
            throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
        }

        var view = CreateView(type);

        OpenView(view);
        try
        {
            if (view.BindingContext is IPopupInitialize<TParameter> initialize)
            {
                initialize.Initialize(parameter);
            }

            if (view.BindingContext is IPopupInitializeAsync<TParameter> initializeAsync)
            {
                await initializeAsync.Initialize(parameter).ConfigureAwait(true);
            }

            var option = optionFactory?.Invoke(type, id);
            var result = await Application.Current!.Windows[0].Page!.ShowPopupAsync<TResult>(view, option).ConfigureAwait(true);
            return result.Result ?? default!;
        }
        finally
        {
            await CloseViewAsync(view).ConfigureAwait(true);
        }
    }

    public async ValueTask CloseAsync()
    {
        var view = BeginClose();
        if (view is not null)
        {
            try
            {
                await NotifyClosingAsync(view).ConfigureAwait(true);
                await Application.Current!.Windows[0].Page!.ClosePopupAsync().ConfigureAwait(true);
            }
            catch
            {
                closingDepth = 0;
                throw;
            }
        }
    }

    public async ValueTask CloseAsync<TResult>(TResult result)
    {
        var view = BeginClose();
        if (view is not null)
        {
            try
            {
                await NotifyClosingAsync(view).ConfigureAwait(true);
                await Application.Current!.Windows[0].Page!.ClosePopupAsync(result).ConfigureAwait(true);
            }
            catch
            {
                closingDepth = 0;
                throw;
            }
        }
    }

    private ContentView? BeginClose()
    {
        if ((views.Count == 0) || (closingDepth == views.Count))
        {
            return null;
        }

        closingDepth = views.Count;
        return views[^1];
    }

    private static async ValueTask NotifyClosingAsync(ContentView view)
    {
        if (view.BindingContext is IPopupClosing closing)
        {
            closing.OnClosing();
        }

        if (view.BindingContext is IPopupClosingAsync closingAsync)
        {
            await closingAsync.OnClosing().ConfigureAwait(true);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Types stored in PopupNavigatorConfig.Register are annotated with DynamicallyAccessedMembers.")]
    private ContentView CreateView([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
    {
        var view = popupFactory.Create(type);

        foreach (var plugin in plugins)
        {
            plugin.Extend(view);
        }

        if (view.BindingContext is IPopupNavigatorAware aware)
        {
            aware.SetPopupNavigator(this);
        }

        return view;
    }

    private void OpenView(ContentView view)
    {
        views.Add(view);

        foreach (var plugin in plugins)
        {
            plugin.OnOpening(view);
        }
    }

    private async ValueTask CloseViewAsync(ContentView view)
    {
        if (closingDepth == views.Count)
        {
            closingDepth = 0;
        }
        else
        {
            await NotifyClosingAsync(view).ConfigureAwait(true);
        }

        views.Remove(view);

        foreach (var plugin in plugins)
        {
            plugin.OnClosed(view);
        }

        view.Behaviors.Clear();
        view.Triggers.Clear();

        if (view.Content is not null)
        {
            Cleanup(view.Content);
        }

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
