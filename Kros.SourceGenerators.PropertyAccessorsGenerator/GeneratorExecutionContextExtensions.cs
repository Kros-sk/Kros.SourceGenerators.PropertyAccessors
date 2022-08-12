using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator
{
    /// <summary>
    /// Extensions for <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    public static class GeneratorExecutionContextExtensions
    {
        private static readonly DiagnosticDescriptor _missingPartialModifier = new DiagnosticDescriptor(
            id: "KROS001",
            title: "Missing partial modifier",
            messageFormat: "A partial modifier is required, property access methods will not be generated",
            category: "Kros.SourceGenerators.PropertyAccessors",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _missingPartialModifierOnProperty = new DiagnosticDescriptor(
            id: "KROS002",
            title: "Missing partial modifier",
            messageFormat: "Some complex public properties are missing a partial modifier, which is required for property " +
                "access methods to correctly support hierarchy",
            category: "Kros.SourceGenerators.PropertyAccessors",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <summary>
        /// Creates warning about missing partial modifier.
        /// </summary>
        /// <param name="context"><see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="classDeclaration">Declaration of class the warning should be shown for.</param>
        public static void ReportMissingPartialModifier(
            this GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration)
            => context.ReportDiagnostic(
                Diagnostic.Create(_missingPartialModifier, classDeclaration.GetLocation()));

        /// <summary>
        /// Creates warning about missing partial modifier on one or more of class properties' defining types.
        /// </summary>
        /// <param name="context"><see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="classDeclaration">Declaration of class the warning should be shown for.</param>
        public static void ReportMissingPartialModifierOnProperty(
            this GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration)
            => context.ReportDiagnostic(
                Diagnostic.Create(_missingPartialModifierOnProperty, classDeclaration.GetLocation()));
    }
}
