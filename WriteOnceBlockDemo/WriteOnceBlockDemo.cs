using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace WriteOnceBlockDemo
{
    public class WriteOnceBlockDemo
    {
        //WriteOnceBlock only stores the first ever value passed to it
        //It passes its value to all the linked targets irrespective of if it has been accepted by the first targetBlock
        //It is similar to broadcast Block except for the behaviour of remembering only first ever value passed to it
        WriteOnceBlock<int> buffer = new WriteOnceBlock<int>(n => n, new DataflowBlockOptions() { EnsureOrdered = true, MaxMessagesPerTask = 5, BoundedCapacity = 1 });
        ActionBlock<int> firstProcessingBlock = new ActionBlock<int>(n => firstProcess(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        ActionBlock<int> secondProcessingBlock = new ActionBlock<int>(n => secondProcess(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        private static void firstProcess(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(6));
            Console.WriteLine("Processing by First Block :" + n);
        }
        private static void secondProcess(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(6));
            Console.WriteLine("Processing by Second Block :" + n);
        }

        public async void Execute()
        {
            //
            buffer.LinkTo(secondProcessingBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            buffer.LinkTo(firstProcessingBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            for (int i = 0; i < 10; i++)
            {
                await buffer.SendAsync(i);
               
                Console.WriteLine("added " + i);
            }

        }
    }
}
