using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class BatchBlockDemo
    {
        BufferBlock<int> buffer = new BufferBlock<int>();
       
        BatchBlock<int> batch = new BatchBlock<int>(10);

        ActionBlock<int[]> batchPrinting = new ActionBlock<int[]>(n => printBatch(n));

        private static void printBatch(int[] n)
        {


            Console.WriteLine("Printing Batch :");
            
            for (int i = 0; i < n.Length; i++)
            {
                Console.Write(n[i] + " ");
            }
            Console.WriteLine();

        }

        public async Task Execute()
        {
            buffer.LinkTo(batch,new DataflowLinkOptions() { PropagateCompletion = true});
            batch.LinkTo(batchPrinting, new DataflowLinkOptions() { PropagateCompletion = true });
            for(int i=1;i<=61;i++)
            {
                await buffer.SendAsync(i);
               
            }
            buffer.Complete();
           
            try
            {
                await batchPrinting.Completion;
                await batchPrinting.Completion.ContinueWith(task => Console.WriteLine("Pipeline completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.InnerException.Message);
            }
            


        }
    }
}
