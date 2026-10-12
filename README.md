# MauiComponents

| Package | Info |
|:-|:-|
| Components.Maui | [![NuGet](https://img.shields.io/nuget/v/Components.Maui.svg)](https://www.nuget.org/packages/Components.Maui) |
| Components.Maui.Resolver | [![NuGet](https://img.shields.io/nuget/v/Components.Maui.Resolver.svg)](https://www.nuget.org/packages/Components.Maui.Resolver) |
| Components.Maui.Controls | [![NuGet](https://img.shields.io/nuget/v/Components.Maui.Controls.svg)](https://www.nuget.org/packages/Components.Maui.Controls) |

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
builder.Services.AddComponentsWiFi();
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

### WiFi

`IWiFiManager` monitors the Wi-Fi connection and scan results (Android only. On iOS `IsSupported` is `false`).

```csharp
wifiManager.StateChanged += (_, _) => Console.WriteLine(wifiManager.Connection?.Ssid);
wifiManager.Enabled = true;
var accessPoints = wifiManager.AccessPoints;
wifiManager.StartScan();
wifiManager.OpenSettings();
```

Requires `ACCESS_FINE_LOCATION` (SSID / BSSID and scan results) and `NEARBY_WIFI_DEVICES` (Android 13+) permissions.

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

## Controls

`Components.Maui.Controls` provides controls and layouts built with MAUI views only.

| Control | Description |
|:-|:-|
| `BottomSheetView` | Sheet from the bottom that opens half expanded or expanded, with drag, fling and backdrop tap |
| `SideDrawer` | Drawer from the left with edge swipe and fling |
| `TabStrip` | Scrolling tab headers with an indicator that follows the page swipe |
| `ChipGroup` | Chips to select one item, in a row or wrapped |
| `SegmentedView` | Segments to select one item with a sliding fill |
| `AccordionView` | Items (`AccordionItem`) that expand and collapse by tapping the header |
| `ShimmerView` | Light band moving over a loading skeleton |
| `StepIndicator` | Dots of the steps and an "n / N" label |
| `MarqueeLabel` | Text that keeps scrolling from right to left |
| `AvatarGroup` | Overlapped avatars with a "+N" count |
| `OverlapPanel` | Layout that overlaps the children by an X / Y offset |
| `StaggeredGrid` | Layout that places items of different heights in the shortest column |
| `VariableSizeWrapPanel` | Tiles in fixed columns with `ColumnSpan` / `RowSpan` |
| `TreeView` | Simple tree of `TreeNode` that expands and collapses |

```xml
<ContentPage xmlns:controls="clr-namespace:MauiComponents.Controls;assembly=MauiComponents.Controls">
    <Grid>
        <controls:ChipGroup ItemsSource="{Binding Categories}"
                            SelectedItem="{Binding SelectedCategory}" />
        <controls:BottomSheetView IsOpen="{Binding IsSheetOpen}">
            <controls:BottomSheetView.SheetContent>
                <Label Text="Sheet" />
            </controls:BottomSheetView.SheetContent>
        </controls:BottomSheetView>
    </Grid>
</ContentPage>
```
