using System;
using System.Threading.Tasks;

namespace DataFLowBlockCancellationDemo
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
            DataFlowBlockCancellation cancellationBlock = new DataFlowBlockCancellation();
            await cancellationBlock.Execute();
            Console.ReadKey();
        }
    }
}
