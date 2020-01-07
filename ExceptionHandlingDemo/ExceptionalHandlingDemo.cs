using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ExceptionHandlingDemo
{
    class ExceptionalHandlingDemo
    {
        public async void  Execute()
        {
            var printAction = new ActionBlock<int>(n => 
            {

                if (n < 0)
                    throw new Exception("test");

                else

                {
                    Thread.Sleep(2000);
                    Console.WriteLine("Action Block Printing"+n);
                }

            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3});

            var firstBlock = new TransformBlock<int,int>(n =>
            {
                //Thread.Sleep(1000);
                return n;
            },
            new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1}

            );
            firstBlock.LinkTo(printAction,new DataflowLinkOptions() { PropagateCompletion = true });
            foreach(var i in new int[] {0,-23,-34,54,56,67,998,689,676})
            {
                firstBlock.Post(i);
            }
            //Thread.Sleep(1000);
            Console.WriteLine("Output count of transform block  :" + firstBlock.OutputCount);
            Console.WriteLine("Input count of action block  :"+printAction.InputCount);



            firstBlock.Complete();
            try
            {
                //Thread.Sleep(5000);
                Console.WriteLine("FirstBlock Input count"+firstBlock.InputCount);
                Console.WriteLine("FirstBlock Output count"+firstBlock.OutputCount);
                Console.WriteLine("Print Block input count "+printAction.InputCount);
                Task.WaitAll(firstBlock.Completion);               
               
            }
            catch(Exception ex)
            {
                
                Console.WriteLine("Inside Catch Block");
                Console.WriteLine("Input Count of Faulted Transform Block:"+firstBlock.InputCount);//will be zero as ques are cleared when faulted
                Console.WriteLine("Output Count of Faulted Transform Block:" + firstBlock.OutputCount);//will be zero as ques are cleared when faulted
                Console.WriteLine(printAction.Completion.Status);//second block completion status will be faulted
               
                Console.WriteLine(ex.InnerException.Message);
            }
            Console.WriteLine("pipeline Executed");
            
            

                
                
                
          }

       
    }
}
