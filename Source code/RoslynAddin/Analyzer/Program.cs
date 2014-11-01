using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace PoorManIDE
{
    class Program
    {
        private const string ToAnalyze =
            @"
            using System;

            namespace HelloWorld
            {
             class Program
             {
                static void Main(string[] args)
                {
                   Console.WriteLine(""Hello, World3!"");
                   Console.Read()
                }
             
            }";

        static void Main(string[] args)
        {
            Console.WriteLine("Parsing...");
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(ToAnalyze);
            var root = (CompilationUnitSyntax)syntaxTree.GetRoot();
            Console.WriteLine("Code parsed");

            DisplayClasses(root);
            DisplayMethods(root);
            var compilation = Compile(syntaxTree);
            SaveToFileOrDisplayErrors(compilation);

            Console.Read();
        }

        private static void DisplayClasses(CompilationUnitSyntax root)
        {
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToArray();
            Console.WriteLine("\nClasses:");
            foreach (var @class in classes)
            {
                Console.WriteLine(@class.Identifier);
            }
        }

        private static void DisplayMethods(CompilationUnitSyntax root)
        {
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            Console.WriteLine("\nMethods:");
            foreach (var method in methods)
            {
                Console.WriteLine(method.Identifier);
            }
        }

        private static CSharpCompilation Compile(SyntaxTree syntaxTree)
        {
            Console.WriteLine("\nCompilation in process...");
            var compilation = CSharpCompilation.Create("test.exe",
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication),
                syntaxTrees: new[] { syntaxTree },
                references: new[] {
                    new MetadataFileReference(typeof(object).Assembly.Location)
            });
            return compilation;
        }

        private static void SaveToFileOrDisplayErrors(CSharpCompilation compilation)
        {
            using (var file = new FileStream("test.exe", FileMode.Create))
            {
                var compilationResult = compilation.Emit(file);
                if (compilationResult.Success)
                {
                    Console.WriteLine("Compilation succesful");
                }
                else
                {
                    Console.WriteLine("Compilation unsuccesful:");
                    foreach (var diagnostic in compilationResult.Diagnostics)
                    {
                        Console.WriteLine(diagnostic.GetMessage());
                    }
                }
            }
        }
    }
}
