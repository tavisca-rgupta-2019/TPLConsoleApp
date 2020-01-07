using System;

namespace BufferBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            BufferBlockDemo demoBlock = new BufferBlockDemo();
            demoBlock.ExecutePipeline();
            //demoBlock.Execute();
            Console.ReadKey();
        }
    }
}
