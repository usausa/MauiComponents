namespace MauiComponents;

using System.Reflection;

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

    public static void AutoRegister<T>(this PopupNavigatorConfig config, IEnumerable<KeyValuePair<T, Type>> source)
    {
        foreach (var pair in source)
        {
            config.Register(pair.Key!, pair.Value);
        }
    }
}
