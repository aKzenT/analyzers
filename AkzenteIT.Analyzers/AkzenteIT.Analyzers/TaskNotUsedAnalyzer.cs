using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AkzenteIT.Analyzers
{
    /// <summary>
    /// Creates a warning when a method returns a task that is not awaited or otherwise used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TaskNotUsedAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AkzenteITAnalyzers";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.ExpressionStatement);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var statement = context.Node as ExpressionStatementSyntax;
            var expression = statement?.Expression as InvocationExpressionSyntax;

            // We are only interested in direct invocations used as a statement
            if (expression == null) return;

            var expressionType = context.SemanticModel.GetTypeInfo(expression).Type;
            if (expressionType.Name == nameof(Task) && expressionType.Equals(context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(Task).FullName)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, expression.GetLocation()));
            }
        }

    }
}

