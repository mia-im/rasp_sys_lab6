using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForApplication
{
    class Program
    {
        static int Max = 1;
        static void Maxnumber(int c)
        {
            Random rnd = new Random();
            int v = rnd.Next(0, c);
            if (Max < v)
            {
                Max = v;
            }
        }
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            Parallel.For(1, 10000000, i =>
            {
                Maxnumber(i);
            });
            long elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine("Время выполнения алгоритма в миллисекундах: {0} ", elapsed);
            Console.WriteLine("Максимальное число: {0} ", Max);
            Console.ReadLine();
        }
    }
}
