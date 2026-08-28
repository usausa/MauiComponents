namespace MauiComponents.Generator.Tests;

using SourceGenerateHelper.Testing;

public sealed class PipelineCacheTest
{
    private const string Source =
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

            public static partial class PopupRegistry
            {
                [PopupSource]
                public static partial IEnumerable<KeyValuePair<PopupId, Type>> ListPopups();
            }
        }
        """;

    private const string UnrelatedSource =
        """
        namespace Other;

        internal sealed class Unrelated;
        """;

    private const string AddedTargetSource =
        """
        namespace Test
        {
            using MauiComponents;

            [Popup(PopupId.Confirm)]
            public sealed class ConfirmPopup
            {
            }
        }
        """;

    // ------------------------------------------------------------
    // Cache
    // ------------------------------------------------------------

    [Fact]
    public void UnrelatedEditKeepsModelCached()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, UnrelatedSource);

        // Assert
        Assert.Equal(result.FirstGeneratedText, result.SecondGeneratedText);
        Assert.NotEmpty(result.OutputReasons);
        Assert.DoesNotContain(result.OutputReasons, static x => x.IsChanged());
    }

    [Fact]
    public void TargetEditRebuildsModel()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, AddedTargetSource);

        // Assert
        Assert.Contains(result.OutputReasons, static x => x.IsChanged());
    }
}
