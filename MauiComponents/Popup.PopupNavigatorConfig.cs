namespace MauiComponents;

using CommunityToolkit.Maui;

public sealed class PopupNavigatorConfig
{
    internal Dictionary<object, Type> PopupTypes { get; } = [];

    public Func<Type, object, PopupOptions>? OptionFactory { get; set; }

    public void Register(object id, Type type)
    {
        PopupTypes[id] = type;
    }
}
