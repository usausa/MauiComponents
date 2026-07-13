namespace MauiComponents.Generator;

using System.Collections.Immutable;
using System.Text;

using MauiComponents.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SourceGenerateHelper;

[Generator]
public sealed class PopupGenerator : IIncrementalGenerator
{
    private const string PopupSourceAttributeName = "MauiComponents.PopupSourceAttribute";
    private const string PopupAttributeName = "MauiComponents.PopupAttribute";

    private const string GenericEnumerableName = "System.Collections.Generic.IEnumerable<T>";
    private const string KeyValuePairName = "System.Collections.Generic.KeyValuePair<TKey, TValue>";
    private const string TypeName = "System.Type";

    // ------------------------------------------------------------
    // Initialize
    // ------------------------------------------------------------

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sourceProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                PopupSourceAttributeName,
                static (node, _) => IsSourceTargetSyntax(node),
                static (context, _) => GetSourceModel(context))
            .Where(static x => x is not null)
            .Collect();

        var popupProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                PopupAttributeName,
                static (node, _) => IsPopupIdTargetSyntax(node),
                static (context, _) => GetPopupIdModel(context))
            .Where(static x => x is not null)
            .Collect();

        context.RegisterSourceOutput(sourceProvider, static (context, sources) => ReportDiagnostics(context, sources));
        context.RegisterSourceOutput(popupProvider, static (context, popups) => ReportDiagnostics(context, popups));

        var models = sourceProvider
            .Combine(popupProvider)
            .SelectMany(static (pair, token) => JoinSourcesWithPopups(pair.Left, pair.Right, token));

        context.RegisterImplementationSourceOutput(models, static (context, model) => Execute(context, model));
    }

    // ------------------------------------------------------------
    // Parser
    // ------------------------------------------------------------

    private static bool IsSourceTargetSyntax(SyntaxNode node) =>
        node is MethodDeclarationSyntax;

    private static Result<SourceModel> GetSourceModel(GeneratorAttributeSyntaxContext context)
    {
        var syntax = context.TargetNode;
        var methodSymbol = (IMethodSymbol)context.TargetSymbol;

        // Validate method style
        if (!methodSymbol.IsStatic || !methodSymbol.IsPartialDefinition)
        {
            return Results.Error<SourceModel>(new DiagnosticInfo(Diagnostics.InvalidMethodDefinition, syntax.GetLocation(), methodSymbol.Name));
        }

        // Validate argument
        if (methodSymbol.Parameters.Length != 0)
        {
            return Results.Error<SourceModel>(new DiagnosticInfo(Diagnostics.InvalidMethodParameter, syntax.GetLocation(), methodSymbol.Name));
        }

        // Validate return type
        if ((methodSymbol.ReturnType is not INamedTypeSymbol returnTypeSymbol) ||
            (returnTypeSymbol.ConstructedFrom.ToDisplayString() != GenericEnumerableName) ||
            (returnTypeSymbol.TypeArguments[0] is not INamedTypeSymbol keyValueTypeSymbol) ||
            (keyValueTypeSymbol.ConstructedFrom.ToDisplayString() != KeyValuePairName) ||
            (keyValueTypeSymbol.TypeArguments[1].ToDisplayString() != TypeName))
        {
            return Results.Error<SourceModel>(new DiagnosticInfo(Diagnostics.InvalidMethodReturnType, syntax.GetLocation(), methodSymbol.Name));
        }

        var containingType = methodSymbol.ContainingType;
        var ns = String.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        return Results.Success(new SourceModel(
            ns,
            containingType.GetClassName(),
            containingType.IsValueType,
            methodSymbol.DeclaredAccessibility,
            methodSymbol.Name,
            returnTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            keyValueTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            keyValueTypeSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
    }

    private static bool IsPopupIdTargetSyntax(SyntaxNode node) =>
        node is ClassDeclarationSyntax;

    private static Result<EquatableArray<PopupIdModel>> GetPopupIdModel(GeneratorAttributeSyntaxContext context)
    {
        var classSymbol = (ITypeSymbol)context.TargetSymbol;

        return Results.Success(new EquatableArray<PopupIdModel>(
            classSymbol.GetAttributes()
                .Where(static x => x.AttributeClass!.ToDisplayString() == PopupAttributeName)
                .Select(attribute => new PopupIdModel(
                    classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    attribute.ConstructorArguments[0].Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    attribute.ConstructorArguments[0].ToCSharpString(),
                    attribute.ConstructorArguments[0].Value!.ToString()))
                .ToArray()));
    }

    private static ImmutableArray<PopupSourceModel> JoinSourcesWithPopups(
        ImmutableArray<Result<SourceModel>> sourceResults,
        ImmutableArray<Result<EquatableArray<PopupIdModel>>> popupResults,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var popupMap = popupResults
            .SelectValue()
            .SelectMany(static x => x)
            .GroupBy(static x => x.PopupIdClassFullName)
            .ToDictionary(static x => x.Key, static x => x.ToArray());

        token.ThrowIfCancellationRequested();

        var builder = ImmutableArray.CreateBuilder<PopupSourceModel>();
        foreach (var source in sourceResults.SelectValue())
        {
            if (popupMap.TryGetValue(source.PopupIdClassFullName, out var popups))
            {
                builder.Add(new PopupSourceModel(source, new EquatableArray<PopupIdModel>(popups)));
            }
        }

        return builder.ToImmutable();
    }

    // ------------------------------------------------------------
    // Generator
    // ------------------------------------------------------------

    private static void ReportDiagnostics<T>(SourceProductionContext context, ImmutableArray<Result<T>> results)
        where T : IEquatable<T>
    {
        foreach (var info in results.SelectError())
        {
            context.ReportDiagnostic(info);
        }
    }

    private static void Execute(SourceProductionContext context, PopupSourceModel model)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builder = new SourceBuilder();
        BuildSource(builder, model.Source, model.Popups);

        var filename = MakeFilename(model.Source.Namespace, model.Source.ClassName, model.Source.MethodName);
        context.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private static void BuildSource(SourceBuilder builder, SourceModel source, IEnumerable<PopupIdModel> popupIds)
    {
        builder.AutoGenerated();
        builder.EnableNullable();
        builder.NewLine();

        // namespace
        if (!String.IsNullOrEmpty(source.Namespace))
        {
            builder.Namespace(source.Namespace);
            builder.NewLine();
        }

        // class
        builder
            .Indent()
            .Append("partial ")
            .Append(source.IsValueType ? "struct " : "class ")
            .Append(source.ClassName)
            .NewLine();
        builder.BeginScope();

        // method
        builder
            .Indent()
            .Append(source.MethodAccessibility.ToText())
            .Append(" static partial ")
            .Append(source.ReturnTypeName)
            .Append(' ')
            .Append(source.MethodName)
            .Append("()")
            .NewLine();
        builder.BeginScope();

        foreach (var popupId in popupIds)
        {
            builder
                .Indent()
                .Append("yield return new ")
                .Append(source.EntryTypeName)
                .Append('(')
                .Append(popupId.PopupIdFullName)
                .Append(", typeof(")
                .Append(popupId.ClassFullName)
                .Append("));")
                .NewLine();
        }

        builder.EndScope();

        builder.EndScope();
    }

    // ------------------------------------------------------------
    // Helper
    // ------------------------------------------------------------

    private static string MakeFilename(string ns, string className, string methodName)
    {
        var buffer = new StringBuilder();

        if (!String.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }

        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        buffer.Append('_');
        buffer.Append(methodName);
        buffer.Append(".g.cs");

        return buffer.ToString();
    }
}
