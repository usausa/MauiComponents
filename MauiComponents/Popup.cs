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

public interface IPopupControllerHandler
{
    void Close(object? value);
}

internal sealed class PopupControllerHandler : IPopupControllerHandler
{
    private readonly Popup popup;

    public PopupControllerHandler(Popup popup)
    {
        this.popup = popup;
    }

    public void Close(object? value) => popup.Close(value);
}

public interface IPopupController
{
    IPopupControllerHandler? Handler { get; set; }
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

public sealed class PopupBind
{
    public static readonly BindableProperty ControllerProperty = BindableProperty.CreateAttached(
        "Controller",
        typeof(IPopupController),
        typeof(PopupBind),
        null,
        propertyChanged: BindChanged);

    public static IPopupController? GetController(BindableObject bindable) =>
        (IPopupController?)bindable.GetValue(ControllerProperty);

    public static void SetController(BindableObject bindable, IPopupController? value) =>
        bindable.SetValue(ControllerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not Popup popup)
        {
            return;
        }

        if (oldValue is not null)
        {
            var controller = PopupBind.GetController(bindable);
            if (controller is not null)
            {
                controller.Handler = null;
            }
        }

        if (newValue is not null)
        {
            var controller = PopupBind.GetController(bindable);
            if (controller is not null)
            {
                controller.Handler = new PopupControllerHandler(popup);
            }
        }
    }
}

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
