namespace MauiComponents.Generator;

using System.Collections.Immutable;

using MauiComponents.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SourceGenerateHelper;

[Generator]
public sealed class PopupGenerator : IIncrementalGenerator
{
    private const string PopupSourceAttributeName = "MauiComponents.PopupSourceAttribute";
    private const string PopupAttributeName = "MauiComponents.PopupAttribute";

    private const string EnumerableName = "System.Collections.Generic.IEnumerable`1";
    private const string KeyValuePairName = "System.Collections.Generic.KeyValuePair`2";
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
        var sourceTreeProvider = context.ForAttributeWithMetadataNameSyntaxTrees(
            PopupSourceAttributeName,
            static (node, _) => IsSourceTargetSyntax(node));

        var popupProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                PopupAttributeName,
                static (node, _) => IsPopupIdTargetSyntax(node),
                static (context, _) => GetPopupIdModel(context))
            .Where(static x => x is not null)
            .Collect();
        var popupTreeProvider = context.ForAttributeWithMetadataNameSyntaxTrees(
            PopupAttributeName,
            static (node, _) => IsPopupIdTargetSyntax(node));

        context.RegisterSourceOutput(
            sourceProvider.Combine(sourceTreeProvider),
            static (context, pair) => context.ReportDiagnostics(pair.Left.SelectError().Concat(FindHintNameCollisions(pair.Left).Values).Distinct(), pair.Right));
        context.RegisterSourceOutput(
            popupProvider.Combine(popupTreeProvider),
            static (context, pair) => context.ReportDiagnostics(pair.Left.SelectError().Distinct(), pair.Right));

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
        var syntax = (MethodDeclarationSyntax)context.TargetNode;
        var methodSymbol = (IMethodSymbol)context.TargetSymbol;

        // Validate method style
        if (!methodSymbol.IsStatic || !methodSymbol.IsPartialDefinition || (methodSymbol.PartialImplementationPart is not null))
        {
            return Results.Error<SourceModel>(new DiagnosticInfo(Diagnostics.InvalidMethodDefinition, syntax.Identifier.GetLocation(), methodSymbol.Name));
        }

        var containingType = methodSymbol.ContainingType;
        var ns = String.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();
        var containingTypes = new EquatableArray<string>(containingType.GetContainingTypes()
            .Append(containingType)
            .Select(static x => x.GetPartialDeclaration())
            .ToArray());
        var hintName = HintNameBuilder.BuildFromType(containingType, methodSymbol.Name);
        var signature = methodSymbol.GetImplementationSignature(syntax);

        var methodName = $"{containingType.ToDisplayString()}.{methodSymbol.Name}";

        Result<SourceModel> Fallback(DiagnosticInfo error) =>
            new(
                new SourceModel(ns, containingTypes, hintName, signature, string.Empty, string.Empty, IsFallback: true, MethodName: methodName),
                new EquatableArray<DiagnosticInfo>([error]));

        // Validate argument
        if (methodSymbol.Parameters.Length != 0)
        {
            return Fallback(new DiagnosticInfo(Diagnostics.InvalidMethodParameter, syntax.Identifier.GetLocation(), methodSymbol.Name));
        }

        // Validate return type
        if ((methodSymbol.ReturnType is not INamedTypeSymbol returnTypeSymbol) ||
            !returnTypeSymbol.HasFullyQualifiedMetadataName(EnumerableName) ||
            (returnTypeSymbol.TypeArguments[0] is not INamedTypeSymbol keyValueTypeSymbol) ||
            !keyValueTypeSymbol.HasFullyQualifiedMetadataName(KeyValuePairName) ||
            !keyValueTypeSymbol.TypeArguments[1].HasFullyQualifiedMetadataName(TypeName))
        {
            return Fallback(new DiagnosticInfo(Diagnostics.InvalidMethodReturnType, syntax.Identifier.GetLocation(), methodSymbol.Name));
        }

        return Results.Success(new SourceModel(
            ns,
            containingTypes,
            hintName,
            signature,
            keyValueTypeSymbol.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNullable),
            keyValueTypeSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            MethodName: methodName));
    }

    private static bool IsPopupIdTargetSyntax(SyntaxNode node) =>
        node is ClassDeclarationSyntax;

    private static Result<EquatableArray<PopupIdModel>> GetPopupIdModel(GeneratorAttributeSyntaxContext context)
    {
        if (!IsReferable((INamedTypeSymbol)context.TargetSymbol))
        {
            return new Result<EquatableArray<PopupIdModel>>(
                EquatableArray<PopupIdModel>.Empty,
                new EquatableArray<DiagnosticInfo>([new DiagnosticInfo(Diagnostics.InvalidPopupClass, ((ClassDeclarationSyntax)context.TargetNode).Identifier.GetLocation(), context.TargetSymbol.ToDisplayString())]));
        }

        var className = context.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var popups = new List<PopupIdModel>();
        foreach (var attribute in context.Attributes)
        {
            if (attribute.TryGetConstructorArgument(0, out var id) && (id.Type is not null) && (id.ToCSharpExpression() is { } expression))
            {
                popups.Add(new PopupIdModel(className, id.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), expression));
            }
        }

        return Results.Success(new EquatableArray<PopupIdModel>(popups.ToArray()));
    }

    private static bool IsReferable(INamedTypeSymbol type)
    {
        if (type.IsFileLocal)
        {
            return false;
        }

        for (var current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected or Accessibility.ProtectedAndInternal)
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, DiagnosticInfo> FindHintNameCollisions(ImmutableArray<Result<SourceModel>> sources)
    {
        var collisions = new Dictionary<string, DiagnosticInfo>(StringComparer.Ordinal);
        var firsts = new Dictionary<string, SourceModel>(StringComparer.OrdinalIgnoreCase);
        foreach (var source in sources.SelectValue().OrderBy(static x => x.HintName, StringComparer.Ordinal))
        {
            if (!firsts.TryGetValue(source.HintName, out var first))
            {
                firsts.Add(source.HintName, source);
            }
            else if ((first.HintName != source.HintName) && !collisions.ContainsKey(source.HintName))
            {
                collisions.Add(source.HintName, new DiagnosticInfo(Diagnostics.HintNameCollision, (Location?)null, source.MethodName, first.MethodName));
            }
        }

        return collisions;
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
            .Distinct()
            .GroupBy(static x => x.PopupIdClassFullName)
            .ToDictionary(static x => x.Key, static x => x.ToArray());

        token.ThrowIfCancellationRequested();

        var collisions = FindHintNameCollisions(sourceResults);
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        var builder = ImmutableArray.CreateBuilder<PopupSourceModel>();
        foreach (var source in sourceResults.SelectValue())
        {
            if (collisions.ContainsKey(source.HintName) || !emitted.Add(source.HintName))
            {
                continue;
            }

            var popups = !source.IsFallback && popupMap.TryGetValue(source.PopupIdClassFullName, out var matched) ? matched : [];
            builder.Add(new PopupSourceModel(source, new EquatableArray<PopupIdModel>(popups)));
        }

        return builder.ToImmutable();
    }

    // ------------------------------------------------------------
    // Generator
    // ------------------------------------------------------------

    private static void Execute(SourceProductionContext context, PopupSourceModel model)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builder = new SourceBuilder();
        BuildSource(builder, model.Source, model.Popups);

        context.AddSource(model.Source.HintName, builder);
    }

    private static void BuildSource(SourceBuilder builder, SourceModel source, EquatableArray<PopupIdModel> popupIds)
    {
        builder.AutoGenerated();
        builder.EnableNullable();
        builder.Disable("CS0612, CS0618");
        builder.NewLine();

        // namespace
        if (!String.IsNullOrEmpty(source.Namespace))
        {
            builder.Namespace(source.Namespace);
            builder.NewLine();
        }

        // class
        foreach (var containingType in source.ContainingTypes)
        {
            builder
                .Indent()
                .Append(containingType)
                .NewLine();
            builder.BeginScope();
        }

        // method
        builder
            .Indent()
            .Append(source.Signature)
            .NewLine();
        builder.BeginScope();

        if (source.IsFallback)
        {
            builder
                .Indent()
                .Append("throw new global::System.InvalidOperationException();")
                .NewLine();
        }
        else if (popupIds.Count == 0)
        {
            builder
                .Indent()
                .Append("yield break;")
                .NewLine();
        }

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

        for (var i = 0; i < source.ContainingTypes.Count; i++)
        {
            builder.EndScope();
        }
    }
}
