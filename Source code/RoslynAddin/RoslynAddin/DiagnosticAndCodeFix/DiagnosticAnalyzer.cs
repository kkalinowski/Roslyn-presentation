using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiagnosticAndCodeFix
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DiagnosticAnalyzer : ISymbolAnalyzer
    {
        public const string DiagnosticId = "DiagnosticAndCodeFix";
        internal const string Description = "Types should be named using camel case standard";
        internal const string MessageFormat = "Type '{0}' is not named using camel case standard";
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SymbolKind> SymbolKindsOfInterest { get { return ImmutableArray.Create(SymbolKind.NamedType); } }

        public void AnalyzeSymbol(ISymbol symbol, Compilation compilation, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var namedType = (INamedTypeSymbol)symbol;

            if (namedType.Name.Any(x => x == '_') || namedType.Name.All(char.IsUpper) || namedType.Name.All(char.IsLower))
            {
                var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                addDiagnostic(diagnostic);
            }
        }
    }
}
