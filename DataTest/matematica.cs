using System;
using System.Linq;
using System.Threading.Tasks;
using Api;
using CSNS.ClientHelper;

namespace Matematica
{
    public static class Mate
    {
        public static int Soma(int x, int y) => x + y;
    }

    public class Matema
    {
        public int X {get;set;}


        public Matema(int _x)
        {
            X = _x;
        }

        public int Soma(int y)
        {
            X += (int)ActHelper.Execute("Retorna", null);

            return X + y;
        }

        public async Task SubtractAsync()
        {
            var y = await ApiExecute.GetID();
            var result = X + y;
            Console.WriteLine($"Result: {result.ToString()}");
        }
    }
}