namespace MauiComponents.Generator.Tests;

using Microsoft.CodeAnalysis;

public class PopupGeneratorTest
{
    // The marker attributes live in the runtime library (net10.0-android/-ios), which this
    // net10.0 test project cannot reference. The generator resolves them by metadata name,
    // so every source below declares them locally.
    private const string Attributes =
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

    //-----------------------------------------------------------------------
    // Basic
    //-----------------------------------------------------------------------

    [Fact]
    public void BasicPopupSourceGeneratesPartialMethod()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(Attributes +
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
            """);

        Assert.Contains("static partial", generated, StringComparison.Ordinal);
        Assert.Contains("ListPopups()", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(global::Test.AlertPopup)", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(global::Test.ConfirmPopup)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void BasicPopupSourceProducesNoCompilationError()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                [Popup(PopupId.Alert)]
                public sealed class AlertPopup
                {
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }

            }
            """);

        Assert.DoesNotContain(diagnostics, static x => x.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void WhenNoMatchingPopupThenNoSourceIsGenerated()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }

            }
            """);

        Assert.Equal(String.Empty, generated);
    }

    //-----------------------------------------------------------------------
    // MC0001 : method must be static partial
    //-----------------------------------------------------------------------

    [Fact]
    public void Mc0001NonPartialMethodEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static IEnumerable<KeyValuePair<PopupId, Type>> ListPopups() => [];
                }

            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "MC0001");
    }

    [Fact]
    public void Mc0001InstanceMethodEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public partial class PopupRegistry
                {
                    [PopupSource]
                    public partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }

            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "MC0001");
    }

    //-----------------------------------------------------------------------
    // MC0002 : method must have no parameter
    //-----------------------------------------------------------------------

    [Fact]
    public void Mc0002MethodWithParameterEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups(int value);
                }

            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "MC0002");
    }

    //-----------------------------------------------------------------------
    // MC0003 : return type must be IEnumerable<KeyValuePair<TId, Type>>
    //-----------------------------------------------------------------------

    [Fact]
    public void Mc0003InvalidReturnTypeEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial IEnumerable<int> ListPopups();
                }

            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "MC0003");
    }

    [Fact]
    public void Mc0003NonEnumerableReturnTypeEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    public static partial int ListPopups();
                }

            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "MC0003");
    }

    //-----------------------------------------------------------------------
    // Valid cases must stay clean
    //-----------------------------------------------------------------------

    [Fact]
    public void ValidDefinitionEmitsNoDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(Attributes +
            """

            namespace Test
            {

                using System;
                using System.Collections.Generic;
                using MauiComponents;

                public enum PopupId
                {
                    Alert
                }

                [Popup(PopupId.Alert)]
                public sealed class AlertPopup
                {
                }

                public static partial class PopupRegistry
                {
                    [PopupSource]
                    internal static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
                }

            }
            """);

        Assert.Empty(diagnostics);
    }
}
