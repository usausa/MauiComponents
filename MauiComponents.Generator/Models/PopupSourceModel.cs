namespace MauiComponents.Generator.Models;

using SourceGenerateHelper;

internal sealed record PopupSourceModel(
    SourceModel Source,
    EquatableArray<PopupIdModel> Popups);
