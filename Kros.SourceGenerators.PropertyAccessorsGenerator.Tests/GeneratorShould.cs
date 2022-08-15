using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Kros.SourceGenerators.PropertyAccessorsGenerator.Tests
{
    [UsesVerify]
    public class GeneratorShould
    {
        [Theory]
        [InlineData("ClassWithPartialModifierAndAttribute")]
        [InlineData("MultipleClassesWithPartialModifierAndAttribute")]
        [InlineData("ClassesWithoutPartialModifierOrAttribute")]
        [InlineData("NestedClassesWithPartial")]
        [InlineData("NestedClassesWithoutPartialWithAttribute")]
        [InlineData("NestedClassesWithoutPartialAndAttribute")]
        [InlineData("SelfReferencingClass")]
        [InlineData("RoundReferencingClasses")]
        [InlineData("MultipleNestedClasses")]
        public Task GenerateMethods(string sourceCodeFile)
        {
            var sourceCode = AssemblyHelper.GetStringFromResourceFileAsync($"{sourceCodeFile}.txt");
            var result = RunGenerator(sourceCode).Results[0].GeneratedSources;

            return Verify(result.Select(r => r.SourceText.ToString()).ToArray())
                .UseParameters(sourceCodeFile);
        }

        private static GeneratorDriverRunResult RunGenerator(string sourceCode)
        {
            var compilation = CreateCompilation(sourceCode);
            GeneratorDriver driver = CSharpGeneratorDriver.Create(new Generator());
            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);

            return driver.GetRunResult();
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}