using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    class BatchedJoinBlockDemo
    {
        BufferBlock<int> idBuffer = new BufferBlock<int>();
        BufferBlock<string> nameBuffer = new BufferBlock<string>();
        BatchedJoinBlock<int,string> batchJoinBlock = new BatchedJoinBlock<int, string>(7);
        ActionBlock<Tuple<IList<int>, IList<string>>> printInfo = new ActionBlock<Tuple<IList<int>, IList<string>>>(batch => printBatch(batch));

        private static void printBatch(Tuple<IList<int>, IList<string>> batch)
        {
            
            Console.WriteLine("Batch :");
            if (batch.Item1.Count != 0)
            {
                Console.WriteLine("Id :");
                foreach (int n in batch.Item1)
                {
                    Console.WriteLine(n);
                }
            }
            if (batch.Item2.Count != 0)
            {
                Console.WriteLine(" Name :");
                foreach (var e in batch.Item2)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task Execute()
        {
           

            idBuffer.LinkTo(batchJoinBlock.Target1, new DataflowLinkOptions() { PropagateCompletion = true });
            nameBuffer.LinkTo(batchJoinBlock.Target2, new DataflowLinkOptions() { PropagateCompletion = true });
            batchJoinBlock.LinkTo(printInfo, new DataflowLinkOptions() { PropagateCompletion = true });

            var populatingIdBuffer = Task.Factory.StartNew(async ()=> {
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(100);
                    await idBuffer.SendAsync(i);
                }

            });
            var populatingNameBuffer = Task.Factory.StartNew(async()=>
            {
                await nameBuffer.SendAsync("John");
                await nameBuffer.SendAsync("Amar");
                await nameBuffer.SendAsync("Akbar");
                await nameBuffer.SendAsync("Joseph");
            });

            await Task.WhenAll(populatingIdBuffer, populatingNameBuffer).ContinueWith(task => {
                idBuffer.Complete();
                nameBuffer.Complete();
            });
            
            try
            {
                await printInfo.Completion;
                await printInfo.Completion.ContinueWith(task => Console.WriteLine("Pipeline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.InnerException.Message);
            }








           
           





        }
        
    }
}
