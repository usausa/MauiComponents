namespace MauiComponents.Generator.Models;

using SourceGenerateHelper;

internal sealed record SourceModel(
    string Namespace,
    EquatableArray<string> ContainingTypes,
    string HintName,
    string Signature,
    string EntryTypeName,
    string PopupIdClassFullName,
    bool IsFallback = false,
    string MethodName = "");
