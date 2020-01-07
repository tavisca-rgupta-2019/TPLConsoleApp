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
        TransformManyBlock<string, char> firstBlock = new TransformManyBlock<string, char>(str => getChars(str), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1,MaxMessagesPerTask = 4,BoundedCapacity = 2});
        ActionBlock<char> secondBlock = new ActionBlock<char>(str => {
            Thread.Sleep(3000);
            Console.WriteLine("Vowel :" + str); }, new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 1,BoundedCapacity = 1 });
        ActionBlock<char> siblingBlock = new ActionBlock<char>(str =>
        {
            Thread.Sleep(3000);
            Console.WriteLine("Consonant :" + str);
        }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, BoundedCapacity = 1 });

        private static IEnumerable<char> getChars(string str)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = str;
            Console.WriteLine("Current Thread Name is "+Thread.CurrentThread.Name);
            Thread.Sleep(3000);
            Console.WriteLine("Processing "+str);
            return str.ToCharArray();
        }
        public async void ExceutePipeline()
        {
            //firstBlock.LinkTo(secondBlock,ch=> "aeiouAEIOU".Contains(ch));
            //firstBlock.LinkTo(siblingBlock, ch => !("aeiouAEIOU".Contains(ch)));
            await firstBlock.SendAsync("This");
            Console.WriteLine("added This");
            await firstBlock.SendAsync("is");
            Console.WriteLine("added is");
            Console.WriteLine("Output Buffer count"+firstBlock.OutputCount);
            await firstBlock.SendAsync("My");
            Console.WriteLine("added My");
            await firstBlock.SendAsync("Empire");
            Console.WriteLine("added Empire");



        }

    }
}
