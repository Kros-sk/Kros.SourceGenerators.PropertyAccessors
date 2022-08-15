using Microsoft.CodeAnalysis;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator
{
    internal class ClassModel
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string FullName { get => $"{Namespace}.{Name}"; }

        public string Modifier { get; set; }

        public IPropertySymbol[] Properties { get; set; }
    }
}
