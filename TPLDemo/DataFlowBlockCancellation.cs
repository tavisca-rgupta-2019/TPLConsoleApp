using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class DataFlowBlockCancellation
    {
        public async Task Execute()
        {   CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            var sourceBlock = new TransformBlock<int, int>(async n => await getNumber(n),new ExecutionDataflowBlockOptions() {CancellationToken = token });
            var buffer = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 , CancellationToken = token });
            var printAction = new ActionBlock<int>(n => 
            {
                Thread.Sleep(1000);
                Console.WriteLine(n);
            }
            ,new ExecutionDataflowBlockOptions() { BoundedCapacity = 2, CancellationToken = token });
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
                await buffer.Completion;
                await Task.WhenAll(printAction.Completion).ContinueWith(task => Console.WriteLine("Pipeline completed successfully"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (TaskCanceledException  ex)
            {                
                Console.WriteLine("The pipeline completed with status "+printAction.Completion.Status);
            }
        }

        private async Task<int> getNumber(int n)
        {
            return await Task.FromResult<int>(n);
        }
    }

}
