namespace MauiComponents.Generator.Tests;

using System.Reflection;

using Microsoft.CodeAnalysis;

public class RobustnessTests
{
    private const string Head =
        """

        namespace Test
        {
            using System;
            using System.Collections.Generic;
            using MauiComponents;

            public enum PopupId
            {
                Alert,
                Confirm
            }

            [Popup(PopupId.Alert)]
            public sealed class AlertPopup
            {
            }

        """;

    [Fact]
    public void IncompletePopupAttributeDoesNotStopGeneration()
    {
        var source = GeneratorTestHelper.Attributes + Head + """
                [Popup]
                public sealed class ConfirmPopup
                {
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }
            }
            """;

        Assert.DoesNotContain("CS8785", GeneratorTestHelper.GetProblemIds(source));
        Assert.Contains("typeof(global::Test.AlertPopup)", GeneratorTestHelper.GetAllGeneratedSource(source), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("public static partial class Outer", "public static partial class PopupRegistry", "public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();")]
    [InlineData("public static partial class Outer", "public partial record PopupRegistry", "public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();")]
    [InlineData("public static partial class Outer", "internal static partial class PopupRegistry", "internal static partial IEnumerable<KeyValuePair<PopupId, Type?>> ListPopups();")]
    public void ImplementationRepeatsDeclaration(string outer, string host, string method)
    {
        var source = GeneratorTestHelper.Attributes + Head + $$"""
                #nullable enable
                {{outer}}
                {
                    {{host}}
                    {
                        [PopupSource]
                        {{method}}
                    }
                }
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void ErrorGeneratesThrowingImplementation()
    {
        var source = GeneratorTestHelper.Attributes + Head + """
                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups(int value);
                }
            }
            """;

        Assert.Equal(["MC0002"], GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void ErrorsCannotBeSuppressed()
    {
        var descriptors = typeof(PopupGenerator).Assembly.GetType("MauiComponents.Generator.Diagnostics", throwOnError: true)!
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(static x => x.PropertyType == typeof(DiagnosticDescriptor))
            .Select(static x => (DiagnosticDescriptor)x.GetValue(null)!)
            .ToList();

        Assert.All(
            descriptors.Where(static x => x.DefaultSeverity == DiagnosticSeverity.Error),
            static x => Assert.Equal([WellKnownDiagnosticTags.NotConfigurable, WellKnownDiagnosticTags.Compiler], x.CustomTags));
    }

    [Theory]
    [InlineData("public static class Outer { [Popup(PopupId.Confirm)] private sealed class ConfirmPopup { } }")]
    [InlineData("[Popup(PopupId.Confirm)] file sealed class ConfirmPopup { }")]
    public void Mc0004PopupThatCannotBeReferredToEmitsDiagnostic(string popup)
    {
        var source = GeneratorTestHelper.Attributes + Head + popup + """

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }
            }
            """;

        Assert.Equal(["MC0004"], GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void PopupOnTwoPartialDeclarationsIsRegisteredOnce()
    {
        var source = GeneratorTestHelper.Attributes + Head + """
                [Popup(PopupId.Confirm)]
                public sealed partial class ConfirmPopup
                {
                }

                [Popup(PopupId.Confirm)]
                public sealed partial class ConfirmPopup
                {
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }
            }
            """;

        Assert.Equal(2, GeneratorTestHelper.GetAllGeneratedSource(source).Split("typeof(global::Test.ConfirmPopup)").Length);
    }

    [Fact]
    public void ObsoletePopupCompilesWithoutWarning()
    {
        var source = GeneratorTestHelper.Attributes + Head + """
                [Obsolete]
                [Popup(PopupId.Confirm)]
                public sealed class ConfirmPopup
                {
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void Mc0005CaseOnlyMethodNamesGenerateTheFirstOnly()
    {
        var source = GeneratorTestHelper.Attributes + Head + """
                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();

                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> listPopups();
                }
            }
            """;

        var problems = GeneratorTestHelper.GetProblemIds(source);

        Assert.Contains("MC0005", problems);
        Assert.DoesNotContain("CS8785", problems);
    }
}
