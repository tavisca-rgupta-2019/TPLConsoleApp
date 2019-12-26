using System;

namespace DataFLowBlockCancellationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFlowBlockCancellation cancellationBlock = new DataFlowBlockCancellation();
            cancellationBlock.Execute();
            Console.ReadKey();
        }
    }
}
