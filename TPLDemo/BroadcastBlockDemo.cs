using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class BroadcastBlockDemo
    {   BroadcastBlock<int> sender = new BroadcastBlock<int>(num => num);
        ActionBlock<int> FirstRecieverBlock = new ActionBlock<int>(num => FirstPrintAction(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1 , BoundedCapacity = 1 });
        ActionBlock<int> SecondRecieverBlock = new ActionBlock<int>(num =>SecondPrintAction(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1 , BoundedCapacity = 1 });
        private static void SecondPrintAction(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("Second Block Printing : " + num);
        }
        public static void FirstPrintAction(int num)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine("First Block Printing "+ num);
        }
        public async Task  Execute()
        {   sender.LinkTo(FirstRecieverBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            sender.LinkTo(SecondRecieverBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            for (int i=0;i<100;i++)
            {
                await sender.SendAsync(i);
                Console.WriteLine("Added "+i);
            }
            Thread.Sleep(1000);
            sender.Complete();
            try
            {
                await Task.WhenAll(FirstRecieverBlock.Completion, SecondRecieverBlock.Completion);
                await Task.WhenAll(FirstRecieverBlock.Completion, SecondRecieverBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.Message);
            }

           
        }
    }
}
