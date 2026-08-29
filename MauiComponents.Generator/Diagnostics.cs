namespace MauiComponents.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidMethodDefinition { get; } = new(
        id: "MC0001",
        title: "Invalid method definition",
        messageFormat: "[PopupSource] method must be partial extension. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodParameter { get; } = new(
        id: "MC0002",
        title: "Invalid method parameter",
        messageFormat: "[PopupSource] method must not have parameters. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodReturnType { get; } = new(
        id: "MC0003",
        title: "Invalid method return type",
        messageFormat: "[PopupSource] return type must be IEnumerable<KeyValuePair<ViewId, Type>>. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
