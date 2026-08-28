namespace MauiComponents.Generator.Tests;

using Microsoft.CodeAnalysis;

public sealed class DiagnosticTest
{
    [Fact]
    public void Mc0001NonPartialMethodEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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

    [Fact]
    public void Mc0002MethodWithParameterEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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

    [Fact]
    public void Mc0003InvalidReturnTypeEmitsDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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

    [Fact]
    public void ValidDefinitionEmitsNoDiagnostic()
    {
        var diagnostics = GeneratorTestHelper.GetDiagnostics(GeneratorTestHelper.Attributes +
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
