using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    class ExceptionalHandlingDemo
    {
        private static TransformBlock<int, int> firstBlock= new TransformBlock<int, int>(n=> getNumber(n), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3,NameFormat = "Transform Block" });
        
        private static int getNumber(int n)
        {
           
                if (n < 0)
                    throw new Exception("test");
                else
                {
                    Thread.Sleep(2000);
                    return n;
                }
                
           
            
        }

        public async Task Execute()
        {
            
            var printAction = new ActionBlock<int>(n =>
            {
                Thread.Sleep(1000);   
                Console.WriteLine(n);
            },
            new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1});
            var secondBlock = new TransformBlock<int, int>(n =>
            {
                Thread.Sleep(100);
                return n;
            }, new ExecutionDataflowBlockOptions() { NameFormat = "second Block" });

            firstBlock.LinkTo(secondBlock, new DataflowLinkOptions() { PropagateCompletion = true});
            secondBlock.LinkTo(printAction,new DataflowLinkOptions() { PropagateCompletion = true});
            foreach(var i in new int[] {0,23,-34,54,56,67,998,689})
            {
                firstBlock.Post(i);
            }

            Thread.Sleep(5000);
            Console.WriteLine("Output count of transform block  :" + firstBlock.OutputCount);
            Console.WriteLine("Input count of action block  :"+printAction.InputCount);
            firstBlock.Complete();


            

            try
            {



                printAction.Completion.Wait();
                await printAction.Completion.ContinueWith(task => Console.WriteLine("Pipeline completed successfully"), TaskContinuationOptions.OnlyOnRanToCompletion);
                

            }
            catch(AggregateException ex)
            {
                
                Console.WriteLine("Inside Catch Block");
                Console.WriteLine("Pipeline completed with status : "+printAction.Completion.Status);
                Console.WriteLine(ex.InnerException.InnerException.Message);
               
               
               
            }
            
            

                
                
                
          }

       
    }
}
