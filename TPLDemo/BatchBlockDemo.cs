using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class BatchBlockDemo
    {   BufferBlock<int> firstBuffer = new BufferBlock<int>();
        BufferBlock<int> secondBuffer = new BufferBlock<int>();
        BatchBlock<int> batch = new BatchBlock<int>(2);
        ActionBlock<int> printAction = new ActionBlock<int>(n => Console.WriteLine("Printing value offered by first buffer postponed by batch block : " + n));
        ActionBlock<int[]> batchPrinting = new ActionBlock<int[]>(n => printBatch(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 });
        private static void printBatch(int[] n)
        {
            Thread.Sleep(1000);
            Console.WriteLine("Printing Batch :");
            for (int i = 0; i < n.Length; i++)
            {
                Console.Write(n[i] + " ");
            }
            Console.WriteLine();
        }
        public async Task Execute()
        {
            firstBuffer.LinkTo(batch, new DataflowLinkOptions() { PropagateCompletion = true });
            firstBuffer.LinkTo(printAction, new DataflowLinkOptions() { PropagateCompletion = true });
            secondBuffer.LinkTo(batch, new DataflowLinkOptions() { PropagateCompletion = true });
            batch.LinkTo(batchPrinting, new DataflowLinkOptions() { PropagateCompletion = true });
            await firstBuffer.SendAsync(1);
            Console.WriteLine("added message " + 1 + " to first buffer");
            Console.WriteLine("adding message 2 to second buffer after 3 seconds");
            await secondBuffer.SendAsync(2);
            Console.WriteLine("added message 2 to second Buffer");
            firstBuffer.Complete();
            secondBuffer.Complete();
            try
            {
                await batchPrinting.Completion;
                await batchPrinting.Completion.ContinueWith(task => Console.WriteLine("Pipeline completed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Exception Occured : " + ex.InnerException.Message);
            }
        }
    }
}










