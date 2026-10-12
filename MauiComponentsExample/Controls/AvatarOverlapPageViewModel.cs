namespace MauiComponentsExample.Controls;

using System.Collections.ObjectModel;
using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public sealed record SamplePerson(string Initial, Color Color);

public sealed partial class AvatarOverlapPageViewModel : ExtendViewModelBase
{
    private const string AvatarImage = "dotnet_bot.png";

    private static readonly SamplePerson[] AllPeople =
    [
        new("A", Color.FromArgb("#2196F3")),
        new("B", Color.FromArgb("#4CAF50")),
        new("C", Color.FromArgb("#FF9800")),
        new("D", Color.FromArgb("#9C27B0")),
        new("E", Color.FromArgb("#F44336")),
        new("F", Color.FromArgb("#009688")),
        new("G", Color.FromArgb("#795548")),
        new("H", Color.FromArgb("#607D8B"))
    ];

    public ObservableCollection<string> Avatars { get; } = [AvatarImage, AvatarImage, AvatarImage, AvatarImage, AvatarImage];

    public ObservableCollection<SamplePerson> People { get; } = [.. AllPeople.Take(5)];

    [ObservableProperty]
    public partial int MaxDisplayed { get; set; } = 3;

    [ObservableProperty]
    public partial double AvatarSize { get; set; } = 48;

    [ObservableProperty]
    public partial double Overlap { get; set; } = 14;

    [ObservableProperty]
    public partial bool ShowCount { get; set; } = true;

    [ObservableProperty]
    public partial int StrokeColorIndex { get; set; }

    [ObservableProperty]
    public partial double OffsetX { get; set; } = 32;

    [ObservableProperty]
    public partial double OffsetY { get; set; } = 20;

    [ObservableProperty]
    public partial bool ReverseZIndex { get; set; }

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public AvatarOverlapPageViewModel()
    {
        AddCommand = MakeDelegateCommand(() =>
        {
            if (People.Count < AllPeople.Length)
            {
                Avatars.Add(AvatarImage);
                People.Add(AllPeople[People.Count]);
            }
        });
        RemoveCommand = MakeDelegateCommand(() =>
        {
            if (People.Count > 0)
            {
                Avatars.RemoveAt(Avatars.Count - 1);
                People.RemoveAt(People.Count - 1);
            }
        });
    }
}
