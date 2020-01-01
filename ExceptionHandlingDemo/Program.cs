using System;
using System.Threading.Tasks;

namespace ExceptionHandlingDemo
{
    class Program
    {
        public static  void  Main(string[] args)
        {
            ExceptionalHandlingDemo demoBlock = new ExceptionalHandlingDemo();
            demoBlock.Execute();
            Console.ReadKey();
        }

       
    }
}
