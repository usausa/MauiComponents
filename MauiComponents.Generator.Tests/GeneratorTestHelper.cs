namespace MauiComponents.Generator.Tests;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper.Testing;

internal static class GeneratorTestHelper
{
    // The runtime library targets net10.0-android/-ios, so it cannot be referenced from this
    // net10.0 test project. The generator matches attributes by metadata name, so each test
    // source declares MauiComponents.PopupAttribute / PopupSourceAttribute itself.
    private static GeneratorTestRunner Runner => GeneratorTestRunner
        .For<PopupGenerator>()
        .WithDiagnosticPrefix("MC");

    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) => Runner.GetDiagnostics(source);

    public static IReadOnlyList<Diagnostic> GetDiagnosticsAll(string source) => Runner.GetDiagnosticsAll(source);

    public static string GetGeneratedSource(string source) => Runner.GetGeneratedSource(source);
}
