namespace MauiComponents;

public sealed class PopupNavigatorConfig
{
    internal Dictionary<object, Type> PopupTypes { get; } = [];

    public void Register(object id, Type type)
    {
        PopupTypes[id] = type;
    }
}
