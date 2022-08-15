using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator
{
    /// <summary>
    /// Receiver for class candidates with <c>PropertyValueProvider</c> attribute.
    /// </summary>
    internal class PropertyValueProviderReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// Attribute name that triggers the receiver.
        /// </summary>
        public const string AttributeName = "PropertyValueProvider";

        private readonly List<ClassDeclarationSyntax> _candidates = new List<ClassDeclarationSyntax>();

        public IEnumerable<ClassDeclarationSyntax> Candidates => _candidates;

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classSyntax && classSyntax.HasAttribute(AttributeName))
            {
                _candidates.Add(classSyntax);
            }
        }
    }
}
