namespace MauiComponentsExample.Controls;

using System.Globalization;

public sealed class ColorIndexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value is int index) && (index > 0) && (index < SampleColors.Values.Count)
            ? SampleColors.Values[index]
            : Color.FromArgb(parameter as string ?? "#00000000");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
