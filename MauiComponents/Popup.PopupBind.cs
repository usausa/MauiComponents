namespace MauiComponents;

using CommunityToolkit.Maui.Views;

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
            var controller = GetController(bindable);
            if (controller is not null)
            {
                controller.Handler = null;
            }
        }

        if (newValue is not null)
        {
            var controller = GetController(bindable);
            if (controller is not null)
            {
                controller.Handler = new PopupControllerHandler(popup);
            }
        }
    }
}
