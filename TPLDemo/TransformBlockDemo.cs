using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class TransformBlockDemo
    {   //Explain maxdegree of parallelism and max messages per task 
        TransformBlock<int, int> firstBlock = new TransformBlock<int, int>(async num => await getNumber(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4,MaxMessagesPerTask = 4 });
        ActionBlock<int> secondBlock = new ActionBlock<int>(num => PrintSecond(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 });
        ActionBlock<int> ThirdBlock = new ActionBlock<int>(num => PrintThird(num), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1,BoundedCapacity = 1 });
        public static void PrintSecond(int num)
        {
            Console.WriteLine("Second Block Printing :" + num);
        }
        public static void PrintThird(int num)
        {
            Console.WriteLine("Third Block Printing :" + num);
        }
         public async Task  Execute()
        {

            firstBlock.LinkTo(secondBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            firstBlock.LinkTo(ThirdBlock,new DataflowLinkOptions() { PropagateCompletion = true});
            for (int i = 0; i < 16; i++)
            {
                await firstBlock.SendAsync<int>(i);
                Console.WriteLine("Added " + i);
            }
            firstBlock.Complete();
            try
            {
                await Task.WhenAll(secondBlock.Completion, ThirdBlock.Completion);
                await Task.WhenAll(secondBlock.Completion, ThirdBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception occured : "+ex.InnerException.Message);
            }

        }
        private static async Task<int> getNumber(int num)
        {
            Console.WriteLine("Execution started for " + num);
            Thread.Sleep(TimeSpan.FromSeconds(6));
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = num.ToString();
            }
            Console.WriteLine("Consumed value "+num+" by thread with name " + Thread.CurrentThread.Name);
            return await Task.FromResult<int>(num);
        }
    }
}
