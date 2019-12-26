using System;

namespace WriteOnceBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteOnceBlockDemo demoBlock = new WriteOnceBlockDemo();
            demoBlock.Execute();
            Console.ReadKey();
        }
    }
}
