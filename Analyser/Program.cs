using Analyser.Helpers;
using Analyser.Static;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Analyser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string filename = "matematica.cs";
            const string path = "C:\\Users\\T-Gamer\\source\\repos\\Analyser\\Analyser\\bin\\Debug\\netcoreapp2.1\\Data";

            string code = Path.Combine(path, filename);

            Loader loader = new Loader(code, "_matematicas");
            await loader.LoadFileAsync();
            var assembly = loader.Assembly;

            CSNSStaticLoader.LoadCode(assembly);

            var soma = CSNSStaticLoader.LoadMethod(assembly, "matematica.Mate.soma");

            var result = soma(2,3);

            var classe = CSNSStaticLoader.InstanceClass(assembly, "matematica.Matema", new object[] { 2 });

            var resultado = classe.soma(4);

            Console.ReadKey();
        }
    }
}
