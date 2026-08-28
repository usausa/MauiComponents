namespace MauiComponents.Generator.Tests;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper.Testing;

internal static class GeneratorTestHelper
{
    public const string Attributes =
        """
        using System;

        namespace MauiComponents
        {
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class PopupAttribute : Attribute
            {
                public PopupAttribute(object id)
                {
                    Id = id;
                }

                public object Id { get; }
            }

            [AttributeUsage(AttributeTargets.Method)]
            public sealed class PopupSourceAttribute : Attribute
            {
            }
        }
        """;

    private static GeneratorTestRunner Runner => GeneratorTestRunner
        .For<PopupGenerator>()
        .WithDiagnosticPrefix("MC");

    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) => Runner.GetDiagnostics(source);

    public static IReadOnlyList<Diagnostic> GetDiagnosticsAll(string source) => Runner.GetDiagnosticsAll(source);

    public static string GetGeneratedSource(string source) => Runner.GetGeneratedSource(source);

    public static IncrementalRunResult RunIncremental(string source, string addedSource) =>
        Runner.WithTracking().RunIncremental(source, addedSource);
}
