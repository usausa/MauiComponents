namespace MauiComponents.Generator;

using System.Collections.Immutable;
using System.Text;

using MauiComponents.Generator.Helpers;
using MauiComponents.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public sealed class PopupGenerator : IIncrementalGenerator
{
    private const string PopupSourceAttributeName = "MauiComponents.PopupSourceAttribute";
    private const string PopupAttributeName = "MauiComponents.PopupAttribute";

    private const string EnumerableName = "System.Collections.Generic.IEnumerable<T>";
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

        context.RegisterImplementationSourceOutput(
            sourceProvider.Combine(popupProvider),
            static (context, provider) => Execute(context, provider.Left, provider.Right));
    }

    // ------------------------------------------------------------
    // Parser
    // ------------------------------------------------------------

    private static bool IsSourceTargetSyntax(SyntaxNode node) =>
        node is MethodDeclarationSyntax;

    private static Result<SourceModel> GetSourceModel(GeneratorAttributeSyntaxContext context)
    {
        var syntax = (MethodDeclarationSyntax)context.TargetNode;
        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not { } methodSymbol)
        {
            return Results.Error<SourceModel>(null);
        }

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
            (returnTypeSymbol.ConstructedFrom.ToDisplayString() != EnumerableName) ||
            (returnTypeSymbol.TypeArguments[0] is not INamedTypeSymbol keyValueTypeSymbol) ||
            (keyValueTypeSymbol.ConstructedFrom.ToDisplayString() != KeyValuePairName) ||
            (keyValueTypeSymbol.TypeArguments[1].ToDisplayString() != TypeName))
        {
            return Results.Error<SourceModel>(new DiagnosticInfo(Diagnostics.InvalidMethodReturnType, syntax.GetLocation(), methodSymbol.Name));
        }

        var containingType = methodSymbol.ContainingType;
        var ns = string.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        return Results.Success(new SourceModel(
            ns,
            containingType.Name,
            containingType.IsValueType,
            methodSymbol.DeclaredAccessibility,
            methodSymbol.Name,
            returnTypeSymbol.ToDisplayString(),
            keyValueTypeSymbol.ToDisplayString(),
            keyValueTypeSymbol.TypeArguments[0].ToDisplayString()));
    }

    private static bool IsPopupIdTargetSyntax(SyntaxNode node) =>
        node is ClassDeclarationSyntax;

    private static Result<EquatableArray<PopupIdModel>> GetPopupIdModel(GeneratorAttributeSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.TargetNode;
        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classSyntax) is not ITypeSymbol classSymbol)
        {
            return Results.Error<EquatableArray<PopupIdModel>>(null);
        }

        return Results.Success(new EquatableArray<PopupIdModel>(
            classSymbol.GetAttributes()
                .Where(static x => x.AttributeClass!.ToDisplayString() == PopupAttributeName)
                .Select(attribute => new PopupIdModel(
                    classSymbol.ToDisplayString(),
                    attribute.ConstructorArguments[0].Type!.ToDisplayString(),
                    attribute.ConstructorArguments[0].ToCSharpString(),
                    attribute.ConstructorArguments[0].Value!.ToString()))
                .ToArray()));
    }

    // ------------------------------------------------------------
    // Generator
    // ------------------------------------------------------------

    private static void Execute(SourceProductionContext context, ImmutableArray<Result<SourceModel>> popupSources, ImmutableArray<Result<EquatableArray<PopupIdModel>>> popupIds)
    {
        foreach (var info in popupSources.SelectPart(static x => x.Error))
        {
            context.ReportDiagnostic(info);
        }
        foreach (var info in popupIds.SelectPart(static x => x.Error))
        {
            context.ReportDiagnostic(info);
        }

        var popupMap = popupIds
            .SelectPart(static x => x.Value)
            .SelectMany(static x => x.ToArray())
            .GroupBy(static x => x.PopupIdClassFullName)
            .ToDictionary(static x => x.Key, static x => x.ToList());

        var builder = new SourceBuilder();
        foreach (var popupSource in popupSources.SelectPart(static x => x.Value))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (popupMap.TryGetValue(popupSource.PopupIdClassFullName, out var popupList))
            {
                builder.Clear();

                BuildSource(builder, popupSource, popupList);

                var filename = MakeFilename(popupSource.Namespace, popupSource.ClassName, popupSource.MethodName);
                var source = builder.ToString();
                context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
            }
        }
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

        if (!string.IsNullOrEmpty(ns))
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
