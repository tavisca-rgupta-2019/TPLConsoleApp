using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class WriteOnceBlockDemo
    {   WriteOnceBlock<int> buffer = new WriteOnceBlock<int>(n => n);
        ActionBlock<int> firstProcessingBlock = new ActionBlock<int>(n => firstProcess(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });
        ActionBlock<int> secondProcessingBlock = new ActionBlock<int>(n => secondProcess(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2});
        private static void firstProcess(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Processing by First Block :" + n);
        }
        private static void secondProcess(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Processing by Second Block :" + n);
        }
        public async Task Execute()
        {   buffer.LinkTo(secondProcessingBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            buffer.LinkTo(firstProcessingBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            for (int i = 0; i < 10; i++)
            {
                bool isAccepted = await buffer.SendAsync(i);
                if(isAccepted==false)
                    Console.WriteLine(i+" was not accepted");
                else
                {
                    Console.WriteLine("added " + i);
                }
            }
            try
            {
                await Task.WhenAll(secondProcessingBlock.Completion, firstProcessingBlock.Completion);
                await Task.WhenAll(secondProcessingBlock.Completion, firstProcessingBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.Message);
            }
        }
    }
}
