namespace MauiComponents.Generator.Tests;

using Microsoft.CodeAnalysis;

public class PopupGeneratorTest
{
    //-----------------------------------------------------------------------
    // Basic
    //-----------------------------------------------------------------------

    [Fact]
    public void BasicPopupSourceGeneratesPartialMethod()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(GeneratorTestHelper.Attributes +
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
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(GeneratorTestHelper.Attributes +
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
        var generated = GeneratorTestHelper.GetGeneratedSource(GeneratorTestHelper.Attributes +
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
    // MC0001
    //-----------------------------------------------------------------------

    //-----------------------------------------------------------------------
    // Valid
    //-----------------------------------------------------------------------
}
