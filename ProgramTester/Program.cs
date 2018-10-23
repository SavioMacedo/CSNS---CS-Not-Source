using CSNS.Helpers;
using CSNS.Static;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProgramTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string path = "C:\\Users\\T-Gamer\\source\\repos\\Analyser\\Analyser\\bin\\Debug\\netcoreapp2.1\\Data";
            
            var assembly = await CSNSMemory.LoadAssemblyAsync(path, "MyAssembly");
            

            var Soma = CSNSStaticLoader.LoadMethod(assembly, "Matematica.Mate.Soma");

            var result = Soma(2, 3);

            Console.WriteLine($"Static method Soma result: {result.ToString()}");

            var classe = CSNSStaticLoader.InstanceClass(assembly, "Matematica.Matema", new object[] { 2 });

            var resultado = classe.Soma(4);

            Console.WriteLine($"Method Soma from instanciated class result: {resultado.ToString()}");

            await classe.SubtractAsync();

            Console.ReadKey();
        }
    }
}
