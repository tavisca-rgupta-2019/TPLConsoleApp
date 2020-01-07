using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TransformBlockDemo
{
    public class TransformBlockDemo
    {
        //Transform outputs single TResult corresponding to each TInput.
        TransformBlock<int, int> firstBlock = new TransformBlock<int, int>(async str => await NumberOfChar(str), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4,MaxMessagesPerTask = 1 });
        ActionBlock<int> secondBlock = new ActionBlock<int>(num => PrintSecond(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        ActionBlock<int> ThirdBlock = new ActionBlock<int>(num => PrintThird(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4, MaxMessagesPerTask = 1,BoundedCapacity = 1 });


        // Most source block types stop offering a message after one target accepts that message
        //Only BroadcastBlock is an exception to the  above behaviour
        //Second and Third Blocks are the example of that
        public static void PrintSecond(int num)
        {
            Console.WriteLine("Second Block Printing :" + num);
        }
        public static void PrintThird(int num)
        {
            Console.WriteLine("Third Block Printing :" + num);
        }

        public async void  Execute()
        {
            //SourceBlock.LinkTo(ITargetBlock,Delegate for conditional passing of message to ITargetBlock)
            //firstBlock.LinkTo(secondBlock);
            //firstBlock.LinkTo(ThirdBlock);
            //Incase none of the Target block accepts the value offered by the souce block, the pipeline will go in a Deadlock state and the next 
            //message from the souce will not be offered to the target block as depicted by the following commented code
            firstBlock.LinkTo(secondBlock, n => n < 0);
            firstBlock.LinkTo(ThirdBlock, n => n > 2);
            //int consumedItem = firstBlock.Receive();
            //Console.WriteLine(consumedItem);


            for (int i = 0; i < 4; i++)
            {
                await firstBlock.SendAsync<int>(i);
                Console.WriteLine("Added " + i);




            }

        }
        private static async Task<int> NumberOfChar(int str)
        {
            Console.WriteLine("Execution started for " + str);
            Thread.Sleep(TimeSpan.FromSeconds(6));
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = str.ToString();
            }
            Console.WriteLine("Consumed " + Thread.CurrentThread.Name + " " + str);
            return await Task.FromResult<int>(str.GetHashCode());

        }
    }
}
