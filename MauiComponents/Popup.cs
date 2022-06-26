namespace MauiComponents;

using System.Reflection;

using CommunityToolkit.Maui.Views;

using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PopupAttribute : Attribute
{
    public object Id { get; }

    public PopupAttribute(object id)
    {
        Id = id;
    }
}

public interface IPopupInitialize<in T>
{
    void Initialize(T parameter);
}

public sealed class PopupBehavior : Behavior<Popup>
{
    // TODO Behavior
    protected override void OnAttachedTo(Popup bindable)
    {
        // TODO
        base.OnAttachedTo(bindable);
    }

    protected override void OnDetachingFrom(Popup bindable)
    {
        // TODO
        base.OnDetachingFrom(bindable);
    }
}

// TODO Controller

public interface IPopupInitializeAsync<in T>
{
    ValueTask Initialize(T parameter);
}

public interface IPopupFactory
{
    Popup Create(Type type);
}

public sealed class DefaultPopupFactory : IPopupFactory
{
    private readonly IServiceProvider provider;

    public DefaultPopupFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public Popup Create(Type type) => (Popup)provider.GetRequiredService(type);
}

public sealed class PopupNavigatorConfig
{
    internal Dictionary<object, Type> PopupTypes { get; } = new();

    public void Register(object id, Type type)
    {
        PopupTypes[id] = type;
    }
}

public static class PopupNavigatorExtensions
{
    public static void AutoRegister(this PopupNavigatorConfig config, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            foreach (var attr in type.GetTypeInfo().GetCustomAttributes<PopupAttribute>())
            {
                config.Register(attr.Id, type);
            }
        }
    }
}

public interface IPopupNavigator
{
    ValueTask<TResult> PopupAsync<TResult>(object id);

    ValueTask<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter);

    ValueTask<object?> PopupAsync(object id);

    ValueTask<object?> PopupAsync<TParameter>(object id, TParameter parameter);
}

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

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(false);

        Cleanup(popup);
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
            await initializeAsync.Initialize(parameter).ConfigureAwait(false);
        }

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(false);

        Cleanup(popup);
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

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(false);

        Cleanup(popup);
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
            await initializeAsync.Initialize(parameter).ConfigureAwait(false);
        }

        var result = await Application.Current!.MainPage!.ShowPopupAsync(popup).ConfigureAwait(false);

        Cleanup(popup);
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

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddComponentsPopup(this IServiceCollection service, Action<PopupNavigatorConfig> action)
    {
        service.AddSingleton(p =>
        {
            var config = new PopupNavigatorConfig();
            action(config);
            return config;
        });
        service.AddSingleton<IPopupFactory, DefaultPopupFactory>();
        service.AddSingleton<IPopupNavigator, PopupNavigator>();
        return service;
    }
}
