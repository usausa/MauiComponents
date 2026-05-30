# MauiComponents

| Package | Info |
|:-|:-|
| Components.Maui | [![NuGet](https://img.shields.io/nuget/v/Components.Maui.svg)](https://www.nuget.org/packages/Components.Maui) |
| Components.Maui.Resolver | [![NuGet](https://img.shields.io/nuget/v/Components.Maui.Resolver.svg)](https://www.nuget.org/packages/Components.Maui.Resolver) |

Component library for .NET MAUI applications.

## Setup

Register the components you need in your MAUI app builder:

```csharp
var builder = MauiApp.CreateBuilder();
builder.Services.AddComponentsDialog();
builder.Services.AddComponentsScreen();
builder.Services.AddComponentsPopup();
builder.Services.AddComponentsLocation();
builder.Services.AddComponentsSpeech();
```

## Features

### Dialog

`IDialog` provides various UI dialogs and notifications.

```csharp
await dialog.InformationAsync("Message");
var confirmed = await dialog.ConfirmAsync("Are you sure?");
var input = await dialog.PromptAsync("Enter value");
var selected = await dialog.SelectAsync("Choose", options);
using (dialog.Loading("Loading...")) { ... }
dialog.Toast("Saved!");
dialog.Snackbar("Item deleted", "Undo", onAction: UndoAction);
```

### Screen

`IScreen` provides screen management.

```csharp
screen.SetOrientation(DisplayOrientation.Landscape);
screen.SetFullscreen(true);
screen.SetScreenBrightness(0.8f);
screen.KeepScreenOn(true);
var stream = await screen.TakeScreenshotAsync();
screen.EnableDetectScreenState(true);
screen.ScreenStateChanged += (_, e) => Console.WriteLine(e.ScreenOn);
```

`IDisplay` monitors display frame rate updates.

### Popup Navigation

`IPopupNavigator` provides popup-based navigation with optional parameters and results.

```csharp
await popupNavigator.PopupAsync(PopupId.Information);
var result = await popupNavigator.PopupAsync<TResult>(PopupId.Select);
await popupNavigator.CloseAsync();
```

### Logger

File logger and Android logger provider integrations for `Microsoft.Extensions.Logging`.

## Smart.Resolver Integration

`Components.Maui.Resolver` provides integration with the [Smart.Resolver](https://github.com/usausa/Smart-Net-Resolver) DI container.
