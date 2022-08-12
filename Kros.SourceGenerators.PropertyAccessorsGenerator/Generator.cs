using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator
{
    /// <summary>
    /// Source generator for property access methods.
    /// </summary>
    [Generator]
    public class Generator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new PropertyValueProviderReceiver());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            string attribute = $@"
using System;

namespace Kros.SourceGenerators
{{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class {PropertyValueProviderReceiver.AttributeName}Attribute: Attribute
    {{
    }}
}}
";
            context.AddSource("Attribute.cs", SourceText.From(Format(attribute), Encoding.UTF8));

            if (context.SyntaxReceiver is PropertyValueProviderReceiver receiver)
            {
                var processedCandidates = new Dictionary<string, bool?>();
                foreach (ClassDeclarationSyntax candidate in receiver.Candidates)
                {
                    TryGeneratePropertyAccessMethods(context, candidate, processedCandidates);
                }
            }
        }

        private static bool? TryGeneratePropertyAccessMethods(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax candidate,
            Dictionary<string, bool?> processedCandidates)
        {
            ClassModel model = GenerateModel(candidate, context.Compilation);
            if (processedCandidates.TryGetValue(model.FullName, out bool? result))
            {
                return result;
            }

            processedCandidates.Add(model.FullName, null);


            if (model.Modifier.Contains("partial"))
            {
                processedCandidates[model.FullName] = true;

                var simpleProperties = new List<IPropertySymbol>();
                var complexProperties = new List<IPropertySymbol>();
                bool allComplexPropertiesOk = true;
                foreach (var property in model.Properties)
                {
                    bool useAsComplexProperty = false;
                    if (property.IsClassObjectProperty())
                    {
                        ClassDeclarationSyntax propertyCandidate = property.GetClassDeclarationSyntax();
                        if (propertyCandidate != null)
                        {
                            bool? propertyProcessingResult =
                                TryGeneratePropertyAccessMethods(context, propertyCandidate, processedCandidates);
                            if (propertyProcessingResult != null)
                            {
                                allComplexPropertiesOk &= propertyProcessingResult.Value;
                                useAsComplexProperty = propertyProcessingResult.Value;
                            }
                        }
                    }

                    if (useAsComplexProperty)
                    {
                        complexProperties.Add(property);
                    }
                    else
                    {
                        simpleProperties.Add(property);
                    }
                }
                if (!allComplexPropertiesOk && candidate.HasAttribute(PropertyValueProviderReceiver.AttributeName))
                {
                    context.ReportMissingPartialModifierOnProperty(candidate);
                }

                string code = Format(GeneratePropertyAccessMethods(model, simpleProperties, complexProperties));
                context.AddSource($"{model.Name}.cs", SourceText.From(code, Encoding.UTF8));

                return true;
            }
            else
            {
                processedCandidates[model.FullName] = false;
                if (candidate.HasAttribute(PropertyValueProviderReceiver.AttributeName))
                {
                    context.ReportMissingPartialModifier(candidate);
                }

                return false;
            }
        }

        private static ClassModel GenerateModel(ClassDeclarationSyntax syntax, Compilation compilation)
        {
            CompilationUnitSyntax root = syntax.GetCompilationUnit();
            SemanticModel classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            return new ClassModel()
            {
                Namespace = root.GetNamespace(),
                Name = syntax.GetClassName(),
                Modifier = syntax.GetClassModifiers(),
                Properties = classSymbol.GetProperties()
            };
        }

        private static string GeneratePropertyAccessMethods(
            ClassModel model,
            List<IPropertySymbol> simpleProperties,
            List<IPropertySymbol> complexProperties)
        {
            var sb = new StringBuilder();

            sb.Append($@"
            using System.Collections.Generic;
            using System.Linq;

            namespace {model.Namespace}
            {{
                {model.Modifier} class {model.Name}
                {{
                    {(complexProperties.Any()
                        ? GenerateGetPropertyValueMethodComplex(simpleProperties, complexProperties)
                        : GenerateGetPropertyValueMethodSimple(simpleProperties))}
                    {GenerateGetAllPropertyValuesMethod(simpleProperties, complexProperties)}
                }}
            }}");

            return sb.ToString();
        }

        private static string GenerateGetPropertyValueMethodSimple(List<IPropertySymbol> properties)
        {
            var sb = new StringBuilder();

            sb.Append(@"
                public object GetPropertyValue(string propertyName)
                {
                    switch (propertyName)
                    {");

            foreach (var property in properties)
            {
                sb.Append($"\ncase nameof({property.Name}):" +
                    $"\nreturn {property.Name};");
            }

            sb.Append(@"
                        default: return null;
                    }
                }");

            return sb.ToString();
        }

        private static string GenerateGetPropertyValueMethodComplex(
            List<IPropertySymbol> simpleProperties,
            List<IPropertySymbol> complexProperties)
        {
            var sb = new StringBuilder();

            sb.Append(@"
                public object GetPropertyValue(string propertyName, char delimiter = '.')
                {
                    if (string.IsNullOrWhiteSpace(propertyName))
                    {
                        return null;
                    }

                    string[] propertyNameParts = propertyName.Split(delimiter);
                    string simplePropertyName = propertyNameParts.First();
                    string propertyNameRemainder = string.Join(delimiter, propertyNameParts.Skip(1));

                   switch (simplePropertyName)
                   {");

            foreach (var property in simpleProperties)
            {
                sb.Append($"\ncase nameof({property.Name}):" +
                    $"\nreturn {property.Name};");
            }
            foreach (var property in complexProperties)
            {
                sb.Append($"\ncase nameof({property.Name}):" +
                    $"\nreturn string.IsNullOrWhiteSpace(propertyNameRemainder)" +
                        $"\n? {property.Name}" +
                        $"\n: {property.Name}?.GetPropertyValue(propertyNameRemainder);");
            }
            sb.Append(@"
                        default: return null;
                    }
                }");

            return sb.ToString();
        }

        private static string GenerateGetAllPropertyValuesMethod(
            List<IPropertySymbol> simpleProperties,
            List<IPropertySymbol> complexProperties)
        {
            var sb = new StringBuilder();

            sb.Append(@"
                public Dictionary<string, object> GetAllPropertyValues()
                    => new Dictionary<string, object>()
                        {");

            for (int i = 0; i < simpleProperties.Count(); i++)
            {
                var property = simpleProperties[i];
                bool isLastProperty = i == simpleProperties.Count() - 1 && !complexProperties.Any();
                sb.Append($"\n[nameof({property.Name})] = {property.Name}{(isLastProperty ? "" : ",")}");
            }
            for (int i = 0; i < complexProperties.Count(); i++)
            {
                var property = complexProperties[i];
                bool isLastProperty = i == complexProperties.Count() - 1;
                sb.Append($"\n[nameof({property.Name})] = {property.Name}?.GetAllPropertyValues(){(isLastProperty ? "" : ",")}");
            }

            sb.Append(@"
                        };");

            return sb.ToString();
        }

        private static string Format(string output)
        {
            var tree = CSharpSyntaxTree.ParseText(output);
            var root = (CSharpSyntaxNode)tree.GetRoot();
            output = root.NormalizeWhitespace().ToFullString();

            return output;
        }
    }
}
