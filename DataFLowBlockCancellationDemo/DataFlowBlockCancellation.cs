using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFLowBlockCancellationDemo
{
    public class DataFlowBlockCancellation
    {



        public async  Task Execute()
        {
            
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            //For cancellation of the block We could issue a cancellation Token and pass it as a ExecutionDataflowBlockOptions Property
            var sourceBlock = new TransformBlock<int,int>(async n => await sleepAction(n));
            var buffer = new BufferBlock<int>(new ExecutionDataflowBlockOptions() { CancellationToken = token});
            var printAction = new ActionBlock<int>(n => Console.WriteLine(n));
            //by setting MaxMessages property to a <int> value, we can ensure that the targetBlock will get unlinked form the source block after
            //passing that much messages to the target
            sourceBlock.LinkTo(buffer,new DataflowLinkOptions() { PropagateCompletion = true});
            //if we will not set the propagate completion property as true, then the cancelation/completion/fault notification will not be 
            //passed to the linked block as a result of which, if we cancel the source Block ,then the ItargetBlock will not complete by itself
            //unless we explicitly call the complete method.
            buffer.LinkTo(printAction);


            //buffer.LinkTo(printAction);

            for (int i=0;i<2;i++)
            {
                await sourceBlock.SendAsync(i);
               
               
            }
            Thread.Sleep(2000);

            
           
            cts.Cancel();
            printAction.Complete();
            
            
            //if we will cancel the block inside a pipeline, then the block preceeding it will not be able to complete but the blocks
            //following the cancelled block will be able to complete automatically if propagateCcompletion is set to true

            try
            {


                //buffer.Completion.Wait();
                 buffer.Completion.ContinueWith(task => Console.WriteLine(buffer.Completion.Status));
                
                printAction.Completion.ContinueWith(task => Console.WriteLine("Print block was also completed"));
            }
            //if we try to wait for the completion of a canceled block it will throw an aggregate exception. for example bufferBlock. 
            catch (AggregateException ex)
            {
                Console.WriteLine("The complete command results in :" + ex.InnerException.Message);
            }
        }

        private async Task<int> sleepAction(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return await Task.FromResult<int>(n);
        }
    }

}
