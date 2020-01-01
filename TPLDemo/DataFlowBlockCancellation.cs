using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class DataFlowBlockCancellation
    {



        public async Task Execute()
        {
            
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            var sourceBlock = new TransformBlock<int,int>(async n => await getNumber(n), new ExecutionDataflowBlockOptions() { CancellationToken = token });
            var buffer = new BufferBlock<int>(new ExecutionDataflowBlockOptions() { CancellationToken = token});
            var printAction = new ActionBlock<int>(n => Console.WriteLine(n), new ExecutionDataflowBlockOptions() { CancellationToken = token });
            sourceBlock.LinkTo(buffer, new DataflowLinkOptions() { PropagateCompletion = true });
            buffer.LinkTo(printAction,new DataflowLinkOptions() { PropagateCompletion = true});


           

            for (int i=0;i<10;i++)
            {
                await sourceBlock.SendAsync(i);
               
               
            }
            Thread.Sleep(3000);

            
           
            cts.Cancel();
           

            try
            {
                Task.WaitAll(sourceBlock.Completion, buffer.Completion, printAction.Completion);
                await Task.WhenAll(sourceBlock.Completion, buffer.Completion, printAction.Completion).ContinueWith(task => Console.WriteLine("Pipeline completed successfully"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            
            catch (AggregateException ex)
            {

                Console.WriteLine("The complete command results in :" + ex.InnerException.Message);
            }
        }

        private async Task<int> getNumber(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return await Task.FromResult<int>(n);
        }
    }

}
