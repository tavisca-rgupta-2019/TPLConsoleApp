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
        BufferBlock<int> buffer = new BufferBlock<int>();
        ActionBlock<int> firstBlock = new ActionBlock<int>(n => secondPrintAction(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 1});
        ActionBlock<int> secondBlock = new ActionBlock<int>(n => firstPrintAction(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 1});
       
        private static void firstPrintAction(int n)
        {
            Thread.Sleep(1000);
            Console.WriteLine("First Block Printing  :"+n);
        }
        public void  Execute()
        {
            for(int i=0;i<10;i++)
            {
                Task<bool> isAccepted = buffer.SendAsync(i);
               
                if (!isAccepted.IsCompleted)
                {
                    Console.WriteLine(i+" was not accepted by the buffer block");
                    
                    Console.WriteLine("Making space for new value");
                    int n = buffer.Receive();
                    Console.WriteLine("Recieved value :" + n);
                    isAccepted.Wait();
                }
                
              
                if(isAccepted.IsCompleted)
                {
                    
                    Console.WriteLine("Buffer Count after adding "+i+" to buffer :" + buffer.Count);
                }
            }
            
            
           
            


        }
        public async Task ExecutePipeline()
        {  
            buffer.LinkTo(firstBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            buffer.LinkTo(secondBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            for (int i = 0; i < 20; i++)
            {
                await buffer.SendAsync(i);

                Console.WriteLine("Buffer Count after posting " + i + ":" + buffer.Count);

            }
            buffer.Complete();
            await Task.WhenAll(secondBlock.Completion,firstBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);

        }
        private static void secondPrintAction(int n)
        {
            Thread.Sleep(1000);
            Console.WriteLine("Second Block Printing :"+n);
        }

    }
}
