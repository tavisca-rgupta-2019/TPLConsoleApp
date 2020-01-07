using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class BufferBlockDemo
    {
        BufferBlock<int> buffer = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 2});
        ActionBlock<int> firstBlock = new ActionBlock<int>(n => secondPrintAction(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 1 });
        ActionBlock<int> secondBlock = new ActionBlock<int>(n => firstPrintAction(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 1,MaxDegreeOfParallelism = 1});
       
        private static void firstPrintAction(int n)
        {
            Thread.Sleep(5000);
            Console.WriteLine("First Block Printing  :"+n);
        }
       public async Task ExecutePipeline()
        {  
            buffer.LinkTo(firstBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            buffer.LinkTo(secondBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            for (int i = 0; i < 20; i++)
            {
               bool isAccepted = await buffer.SendAsync(i);
                if(isAccepted==false)
                    Console.WriteLine("Message was not accepted");
                Console.WriteLine("Buffer Count after posting " + i + ":" + buffer.Count);
            }
            buffer.Complete();
            try
            {
                await Task.WhenAll(secondBlock.Completion, firstBlock.Completion);
                await Task.WhenAll(secondBlock.Completion, firstBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.Message);
            }
        }
        private static void secondPrintAction(int n)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Second Block Printing :"+n);
        }
    }
}
