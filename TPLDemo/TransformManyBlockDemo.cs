using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class TransformManyBlockDemo
    {
       
        TransformManyBlock<string, char> firstBlock = new TransformManyBlock<string, char>(str => getChars(str), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4,MaxMessagesPerTask = 4});
        ActionBlock<char> secondBlock = new ActionBlock<char>(str => Console.WriteLine("Vowel :"+str), new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 1 });
        ActionBlock<char> siblingBlock = new ActionBlock<char>(str => Console.WriteLine("Consonant :" + str), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 });

        private static IEnumerable<char> getChars(string str)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = str;
            Console.WriteLine("Current Thread Name is "+Thread.CurrentThread.Name);
            Thread.Sleep(3000);
            return str.ToCharArray();
        }
        public async Task ExceutePipeline()
        {
            firstBlock.LinkTo(secondBlock,new DataflowLinkOptions() { PropagateCompletion=true},ch=> "aeiouAEIOU".Contains(ch));
            firstBlock.LinkTo(siblingBlock, new DataflowLinkOptions() { PropagateCompletion = true},ch => !("aeiouAEIOU".Contains(ch)));
            firstBlock.Post("This");
            firstBlock.Post("is");
            firstBlock.Post("My");
            firstBlock.Post("Empire");
            firstBlock.Complete();

            try
            {
                Task.WaitAll(secondBlock.Completion, siblingBlock.Completion);
                await Task.WhenAll(secondBlock.Completion, siblingBlock.Completion).ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception occured : "+ex.InnerException.Message);
            }



        }

    }
}
