using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator
{
    /// <summary>
    /// Extensions for Roslyn classes.
    /// </summary>
    internal static class RoslynExtensions
    {
        /// <summary>
        /// Retrieves compilation node for specified <paramref name="syntaxNode"/>.
        /// </summary>
        /// <param name="syntaxNode">Syntax node.</param>
        /// <returns>Compilation node.</returns>
        public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode)
            => syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

        /// <summary>
        /// Retrieves class name.
        /// </summary>
        /// <param name="classDeclaration">Class declaration node.</param>
        /// <returns>Class name.</returns>
        public static string GetClassName(this ClassDeclarationSyntax classDeclaration)
            => classDeclaration.Identifier.Text;

        /// <summary>
        /// Retrieves class modifiers.
        /// </summary>
        /// <param name="classDeclaration">Class declaration node.</param>
        /// <returns>Class modifiers.</returns>
        public static string GetClassModifiers(this ClassDeclarationSyntax classDeclaration)
            => classDeclaration.Modifiers.ToFullString().Trim();

        /// <summary>
        /// Checks whether class has an attribute with <paramref name="attributeName"/>.
        /// </summary>
        /// <param name="classDeclaration">Class declaration node.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns><c>true</c>, if class contains attribute with specified name; otherwise <c>false</c>.</returns>
        public static bool HasAttribute(this ClassDeclarationSyntax classDeclaration, string attributeName)
            => classDeclaration?.AttributeLists.Count > 0
                && classDeclaration
                    .AttributeLists
                       .SelectMany(SelectWithAttributes(attributeName))
                       .Any();

        /// <summary>
        /// Retrieves namespace for compilation.
        /// </summary>
        /// <param name="root">Compilation node.</param>
        /// <returns>Namespace.</returns>
        public static string GetNamespace(this CompilationUnitSyntax root)
            => root.ChildNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name
                .ToString();

        private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> SelectWithAttributes(string attributeName)
            => l => l?.Attributes.Where(a => (a.Name as IdentifierNameSyntax)?.Identifier.Text == attributeName);

        /// <summary>
        /// Retrieves type and all its ancestor types.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Type and its ancestor types.</returns>
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            ITypeSymbol current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Retrieves all members from type (including inherited members).
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Symbols for members.</returns>
        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
            => type.GetBaseTypesAndThis().SelectMany(n => n.GetMembers());

        /// <summary>
        /// Retrieves all properties for specified type (including inherited).
        /// </summary>
        /// <param name="symbol">Type symbol.</param>
        /// <returns>Collection of property symbols.</returns>
        public static IPropertySymbol[] GetProperties(this INamedTypeSymbol symbol)
            => symbol.GetAllMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .OfType<IPropertySymbol>()
                .ToArray();

        /// <summary>
        /// Checks whether property's defining type is a class.
        /// </summary>
        /// <param name="symbol">Property symbol.</param>
        /// <returns><c>true</c>, if property's defining type is a class; otherwise <c>false</c>.</returns>
        /// <remarks>Omits <c>string</c> type.</remarks>
        public static bool IsClassObjectProperty(this IPropertySymbol symbol)
            => symbol.Type.TypeKind == TypeKind.Class && symbol.Type.Name.ToLower() != "string";

        /// <summary>
        /// Retrieves class declaration for property's defining type.
        /// </summary>
        /// <param name="symbol">Property symbol.</param>
        /// <returns>Class declaration node; <c>null</c>, if property's defining type is not a class or
        /// if the defining type's declaration could not be found.</returns>
        public static ClassDeclarationSyntax GetClassDeclarationSyntax(this IPropertySymbol symbol)
        {
            var location = symbol.Type.Locations.FirstOrDefault();
            if (location != null)
            {
                var node = location.SourceTree?.GetRoot()?.FindNode(location.SourceSpan);
                if (node != null && node is ClassDeclarationSyntax clasSyntax)
                {
                    return clasSyntax;
                }
            }

            return null;
        }
    }
}
