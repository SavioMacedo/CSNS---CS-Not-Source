using CSNS.Static;
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

namespace CSNS.Helpers
{
    public class Loader
    {
        public string PathLocation { get; set; }
        public string AssemblyName { get; set; }
        public Assembly Assembly { get; set; }

        public Loader(string _path, string _name = "")
        {
            PathLocation = _path;

            if (string.IsNullOrEmpty(_name))
                AssemblyName = Path.GetFileNameWithoutExtension(PathLocation);
            else
                AssemblyName = _name;
        }

        private async Task<SyntaxTree> LoadSyntaxTreeAsync(string path)
        {
            string code = await File.ReadAllTextAsync(path);
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(code);
            var diagnostics = syntaxTree.GetDiagnostics();

            if (diagnostics.Any())
            {
                var errors = diagnostics.Where(obj => obj.Severity == DiagnosticSeverity.Error);

                if (errors.Any())
                {
                    var error = string.Join(" \n", diagnostics.Select(a => $"{a.WarningLevel.ToString()} - {a.GetMessage()}"));
                    throw new Exception(error);
                }
            }

            return syntaxTree;
        }

        private async Task<IEnumerable<SyntaxTree>> LoadSyntaxTreeFolderAsync(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            FileInfo[] files = directoryInfo.GetFiles("*.cs");

            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            foreach(var file in files)
            {
                var syntaxTree = await LoadSyntaxTreeAsync(file.FullName);
                syntaxTrees.Add(syntaxTree);
            }

            return syntaxTrees;
        }

        public async Task LoadAsync(Type[] types = null)
        {
            SyntaxTree[] syntaxTrees;

            FileAttributes fileAttr = File.GetAttributes(PathLocation);

            if (fileAttr.HasFlag(FileAttributes.Directory))
                syntaxTrees = (await LoadSyntaxTreeFolderAsync(PathLocation)).ToArray();
            else
                syntaxTrees = new SyntaxTree[] { await LoadSyntaxTreeAsync(PathLocation) };

            var callerAssembly = Assembly.GetEntryAssembly();
            ClientHelper.ActHelper.Commands = callerAssembly.GetType("Commands");

            var locationNetCore = typeof(System.Net.Http.HttpClient).Assembly.Location;

            var pathNetCore = @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.1.0\ref\netcoreapp2.1";

            var directoryInfo = new DirectoryInfo(pathNetCore);
            FileInfo[] files = directoryInfo.GetFiles("*.dll");

            List<MetadataReference> metadataReferences = new List<MetadataReference>();

            foreach (var file in files)
            {
                metadataReferences.Add(MetadataReference.CreateFromFile(file.FullName));
            }

            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(CSNS.ClientHelper.ActHelper).Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(Newtonsoft.Json.JsonConvert).Assembly.Location));
            
            //Next work, try to think a way to run References from outside.
            var compilation = CSharpCompilation.Create($"{AssemblyName}.dll", syntaxTrees, metadataReferences, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var result = compilation.Emit(memoryStream);

                if(!result.Success)
                {
                    var diagnostics = result.Diagnostics.Where(obj => obj.Severity == DiagnosticSeverity.Error);

                    if (diagnostics.Any())
                    {
                        var error = string.Join(" \n", diagnostics.Select(a => $"{a.WarningLevel.ToString()} - {a.GetMessage()}"));
                        throw new Exception(error);
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                Assembly = ReloadAssembly(memoryStream);
            }

            CSNSStaticLoader.LoadCode(Assembly);
        }

        public Assembly ReloadAssembly(MemoryStream memoryStream)
        {
            return AssemblyLoadContext.Default.LoadFromStream(memoryStream);
        }

    }
}
