namespace MauiComponentsExample.Dialogs;

using System;
using System.Collections.Generic;

using MauiComponents;

public static partial class ViewRegistry
{
    [PopupSource]
    public static partial IEnumerable<KeyValuePair<DialogId, Type>> DialogSource();
}
