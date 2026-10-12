namespace MauiComponentsExample.Controls;

using System.Windows.Input;

public sealed record SampleAction(string Text, ICommand Command);
