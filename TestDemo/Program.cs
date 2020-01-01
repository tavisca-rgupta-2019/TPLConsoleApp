using System;
using System.Threading.Tasks;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Caller().Wait();
            Console.ReadKey();
        }

        static async Task Caller()
        {
            DemoBlock demoBlock = new DemoBlock();
            await demoBlock.Execute();
            Console.ReadKey();
        }
    }
}
