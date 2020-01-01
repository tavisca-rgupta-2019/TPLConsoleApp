using System;
using System.Threading.Tasks;

namespace BroadcastBlockDemo
{
    class Program
    {
        public static void Main(string[] args)
        {


            BroadcastAndActionBlockDemo dataFlowBlocksDemo = new BroadcastAndActionBlockDemo();
            dataFlowBlocksDemo.Execute();
            Console.ReadKey();
        }

       
    }
}
