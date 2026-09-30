namespace MauiComponents;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class PopupAttribute : Attribute
{
    public object Id { get; }

    public PopupAttribute(object id)
    {
        Id = id;
    }
}
