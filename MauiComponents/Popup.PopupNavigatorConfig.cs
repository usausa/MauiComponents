namespace MauiComponents;

using CommunityToolkit.Maui;

public sealed class PopupNavigatorConfig
{
    private static readonly PopupOptions DefaultOptions = new()
    {
        CanBeDismissedByTappingOutsideOfPopup = false,
        Shadow = null,
        Shape = null
    };

    internal Dictionary<object, Type> PopupTypes { get; } = [];

    public Func<Type, object, PopupOptions> OptionFactory { get; set; } = (_, _) => DefaultOptions;

    public void Register(object id, Type type)
    {
        PopupTypes[id] = type;
    }
}
