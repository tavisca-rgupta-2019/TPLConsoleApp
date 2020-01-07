
using System;
using System.Threading.Tasks;

namespace TPLDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Caller().Wait();
            Console.ReadKey();
        }


        private static async Task Caller()
        {
            Console.WriteLine("Enter one of the following Block choice for demo :");
            Console.WriteLine("1  BufferBlock\n2  BroadcastBlock\n3  WriteOnceBlock\n4  TransformBlock\n5  TransformManyBlock\n6  BatchBlock\n" +
                "7  JoinBlock\n8  BatchedJoinBlock\n9  Cancellation of Block\n10 Exception Handling");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch(choice)
            {
                case 1: var bufferDemo = new BufferBlockDemo();
                        await bufferDemo.ExecutePipeline();
                        break;
                case 2: var broadcastDemo = new BroadcastBlockDemo();
                        await broadcastDemo.Execute();
                        break;
                case 3: var writeOnceBlockDemo = new WriteOnceBlockDemo();
                        await writeOnceBlockDemo.Execute();
                        break;
                case 4: var transformBlockDemo = new TransformBlockDemo();
                        await transformBlockDemo.Execute();
                        break;
                case 5: var transformManyBlockDemo = new TransformManyBlockDemo();
                        await transformManyBlockDemo.ExceutePipeline();
                        break;
                case 6: var batchBlockDemo = new BatchBlockDemo();
                        await batchBlockDemo.Execute();
                        break;
                case 7: var joinBlockDemo = new JoinBlockDemo();
                        await joinBlockDemo.Execute();
                        break;
                case 8: var batchedJoinBlock = new BatchedJoinBlockDemo();
                        await batchedJoinBlock.Execute();
                        break;
                case 9: var cancellationDemo = new DataFlowBlockCancellation();
                        await cancellationDemo.Execute();
                        break;
                case 10: var exceptionHandlingDemo = new ExceptionalHandlingDemo();
                        await exceptionHandlingDemo.Execute();
                        break;
                default: Console.WriteLine("Invalid Choice");
                        break;
            }
        }
    }
}
