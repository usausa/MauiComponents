namespace MauiComponents;

public sealed class PopupNavigatorConfig
{
    internal Dictionary<object, Type> PopupTypes { get; } = new();

    public void Register(object id, Type type)
    {
        PopupTypes[id] = type;
    }
}
