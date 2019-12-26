using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace TransformManyBlockDemo
{
    public class TransformManyBlockDemo
    {
        //TransformManyBlock takes a single input and give multiple Outputs corresponding to that input
        //NOTE: Output Buffer of TransformManyBlock post the message to the next linked message as one by one element
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
        public void ExceutePipeline()
        {
            firstBlock.LinkTo(secondBlock,ch=> "aeiouAEIOU".Contains(ch));
            firstBlock.LinkTo(siblingBlock, ch => !("aeiouAEIOU".Contains(ch)));
            firstBlock.Post("This");
            firstBlock.Post("is");
            firstBlock.Post("My");
            firstBlock.Post("Empire");



        }

    }
}
