using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Analyser.Helpers
{
    public class Loader
    {
        public string PathLocation { get; set; }
        public string AssemblyName { get; set; }
        public Assembly Assembly { get; set; }

        public Loader(string _path, string _name = "")
        {
            PathLocation = _path;

            if (String.IsNullOrEmpty(_name))
                AssemblyName = Path.GetFileNameWithoutExtension(PathLocation);
            else
                AssemblyName = _name;
        }

        public async Task LoadFileAsync()
        {
            string code = await File.ReadAllTextAsync(PathLocation);

            var tree = SyntaxFactory.ParseSyntaxTree(code);

            var diagnostics = tree.GetDiagnostics();

            if (diagnostics.Any())
            {
                foreach (var diag in diagnostics)
                {
                    Console.WriteLine($"{diag.GetMessage()} {diag.Location.GetLineSpan()}");
                }

                if(diagnostics.Where(obj => obj.Severity == DiagnosticSeverity.Error).Any())
                    throw new Exception("Errors");
            }

            //var a = Assembly.GetLoadedModules();

            var compilation = CSharpCompilation.Create($"{AssemblyName}.dll", new SyntaxTree[] { tree }, new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            }, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var result = compilation.Emit(memoryStream);

                if(!result.Success)
                {
                    diagnostics = result.Diagnostics;

                    foreach(var diagnostic in diagnostics)
                    {
                        Console.WriteLine($"{diagnostic.GetMessage()} {diagnostic.Location.GetLineSpan()}");
                    }

                    if (diagnostics.Where(obj => obj.Severity == DiagnosticSeverity.Error).Any())
                        throw new Exception("Errors");
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                Assembly = ReloadAssembly(memoryStream);
            }
        }

        public Assembly ReloadAssembly(MemoryStream memoryStream)
        {
            return AssemblyLoadContext.Default.LoadFromStream(memoryStream);
        }

    }
}
