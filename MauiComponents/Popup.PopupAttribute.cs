namespace MauiComponents;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PopupAttribute : Attribute
{
    public object Id { get; }

    public PopupAttribute(object id)
    {
        Id = id;
    }
}
