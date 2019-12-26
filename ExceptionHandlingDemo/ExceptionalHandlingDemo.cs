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
        public void Execute()
        {
            var firstBlock = new TransformBlock<int, int>(n => 
            {

                if (n < 0)
                    throw new Exception("test");

                else

                {
                    Thread.Sleep(2000);
                    return n;
                }

            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3});

            var printAction = new ActionBlock<int>(n =>
            {
                Thread.Sleep(1000);   
                Console.WriteLine(n);
            },
            new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1}

            );
            firstBlock.LinkTo(printAction,new DataflowLinkOptions() { PropagateCompletion = true });
            foreach(var i in new int[] {0,23,-34,54,56,67,998,689})
            {
                firstBlock.Post(i);
            }
            //Thread.Sleep(1000);
            Console.WriteLine("Output count of transform block  :" + firstBlock.OutputCount);
            Console.WriteLine("Input count of action block  :"+printAction.InputCount);


            

            try
            {
                printAction.Completion.Wait();
               
            }
            catch(Exception ex)
            {
                //whenever an exception is thrown in a block, it enters a faulted state and do the following:
                //it should stop processing additional messages.
                //It should clear its message queues, both input and output.
                //It should fault its Completion task.
                //If it is a target:
                //It should decline any further incoming messages.
                //It should release any reserved messages.
                //Optionally, it may reserve and then release any postponed messages to notify the sources they will never be needed.
                //If it is a source:
                //    It should stop offering messages.
                //    It should transfer the exception to any targets currently linked where those links were created with DataflowLinkOptions.PropagateCompletion == true.
                //    It should unlink from currently linked targets
                Console.WriteLine("Inside Catch Block");
                Console.WriteLine("Input Count of Faulted Transform Block:"+firstBlock.InputCount);//will be zero as ques are cleared when faulted
                Console.WriteLine("Output Count of Faulted Transform Block:" + firstBlock.OutputCount);//will be zero as ques are cleared when faulted
                Console.WriteLine(printAction.Completion.Status);//second block completion status will be faulted
               
                Console.WriteLine(ex.InnerException.Message);
            }
            
            

                
                
                
          }

       
    }
}
