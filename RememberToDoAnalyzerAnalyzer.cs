using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyzerTools.RememberToDo
{
	[DiagnosticAnalyzer (LanguageNames.CSharp)]
	public class RememberToAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "RememberToDoWarning";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor (
			id: DiagnosticId,
			title: "RememberTo Warning",
			messageFormat: "{0}",
			category: "Usage",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true
		);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create (Rule);

		public override void Initialize (AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution ();

			// We'll check attributes on symbol declarations (methods, classes, properties, constructors)
			context.RegisterSymbolAction (AnalyzeSymbol, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property);
		}

		private static void AnalyzeSymbol (SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			// Determine if this compilation is Release or not
			bool isRelease = context.Compilation.Options.OptimizationLevel == OptimizationLevel.Release;

			// Check each attribute
			foreach (var attributeData in symbol.GetAttributes ())
			{
				// Confirm that the attribute class is "RememberToAttribute"
				if (attributeData.AttributeClass?.Name == nameof (RememberToAttribute))
				{
					// Try to retrieve constructor arguments
					//  1) message (string)
					//  2) emitOnDebug (bool, default=false)
					string messageArg = (attributeData.ConstructorArguments.Length > 0)
						? attributeData.ConstructorArguments[0].Value?.ToString () ?? string.Empty
						: string.Empty;

					bool emitOnDebug = false;
					if (attributeData.ConstructorArguments.Length > 1)
					{
						var secondArgValue = attributeData.ConstructorArguments[1].Value;
						emitOnDebug = secondArgValue is bool boolArg ? boolArg : false;
					}

					// We only emit if either:
					//  - We're in Release mode, OR
					//  - The user said "emitOnDebug = true"
					bool shouldEmit = isRelease || emitOnDebug;
					if (shouldEmit)
					{
						// Create and report the diagnostic
						var diagnostic = Diagnostic.Create (Rule, symbol.Locations[0], messageArg);
						context.ReportDiagnostic (diagnostic);
					}
				}
			}
		}
	}
}
