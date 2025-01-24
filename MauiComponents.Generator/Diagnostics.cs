namespace MauiComponents.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidMethodDefinition => new(
        id: "MC0001",
        title: "Invalid method definition",
        messageFormat: "Method must be partial extension. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodParameter => new(
        id: "MC0002",
        title: "Invalid method parameter",
        messageFormat: "Parameter count must be nothing. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodReturnType => new(
        id: "MC0003",
        title: "Invalid method return type",
        messageFormat: "Return type must be IEnumerable<KeyValuePair<ViewId, Type>>. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
