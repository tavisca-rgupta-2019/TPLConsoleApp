using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class BroadcastBlockDemo
    {
       
        BroadcastBlock<int> sender = new BroadcastBlock<int>(num => Distribute(num));
        ActionBlock<int> OddRecieverBlock = new ActionBlock<int>(num => PrintOdd(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1 , BoundedCapacity = 1 });
        ActionBlock<int> EvenRecieverBlock = new ActionBlock<int>(num => PrintEven(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1 , BoundedCapacity = 1 });
       
        private static void PrintEven(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Even "+num);
        }

        private static int Distribute(int num)
        {
            
            return num;
        }

        public static void PrintOdd(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Odd "+ num);
        }

        public async Task  Execute()
        {
            sender.LinkTo(OddRecieverBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            sender.LinkTo(EvenRecieverBlock,new DataflowLinkOptions() { PropagateCompletion = true});


            for (int i=0;i<100;i++)
            {
                await sender.SendAsync(i);
                
               
                
                Console.WriteLine("Added "+i);
               
            }
            Thread.Sleep(1000);
            sender.Complete();
            try
            {
                Task.WaitAll(OddRecieverBlock.Completion, EvenRecieverBlock.Completion);
                await Task.WhenAll(OddRecieverBlock.Completion, EvenRecieverBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.Message);
            }

           
        }
    }
}
