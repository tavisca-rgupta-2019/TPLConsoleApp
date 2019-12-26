using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BroadcastBlockDemo
{
    public class BroadcastAndActionBlockDemo
    {
        //Broadcast block overwrites the input values irrespective of whether the exisiting value has been passed on to the next block or not
        BroadcastBlock<int> sender = new BroadcastBlock<int>(num => Distribute(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1,MaxMessagesPerTask = 1});
        ActionBlock<int> OddRecieverBlock = new ActionBlock<int>(num => PrintOdd(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        ActionBlock<int> EvenRecieverBlock = new ActionBlock<int>(num => PrintEven(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        //The value will be passed to all the linked targets irrespective of whether it was accepted by the first block or not.

        private static void PrintEven(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Even "+num);
        }

        private static int Distribute(int num)
        {
            return num;
        }

        public static void PrintOdd(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Odd "+ num);
        }

        public async Task Execute()
        {
            sender.LinkTo(OddRecieverBlock);
            sender.LinkTo(EvenRecieverBlock);


            for (int i=0;i<100;i++)
            {
                await sender.SendAsync(i);
               
                
                Console.WriteLine("Added "+i);
               
            }
            //only 0 and 99 will get printed as all other values get overwritten in the broadcast block by its next integer value 99 being the last

           
        }
    }
}
