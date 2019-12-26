using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BufferBlockDemo
{
    public class BufferBlockDemo
    {
        BufferBlock<int> buffer = new BufferBlock<int>(new DataflowBlockOptions() {EnsureOrdered = true});
        ActionBlock<int> printEvenBlock = new ActionBlock<int>(n => printEven(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 2});
        ActionBlock<int> printOddBlock = new ActionBlock<int>(n => printOdd(n), new ExecutionDataflowBlockOptions() { BoundedCapacity = 2});
        //If we did not mention the bounded capacity of a buffer , it can store as many values as required to fill up the memory.
       
        private static void printOdd(int n)
        {
            Console.WriteLine("Printing Odd :"+n);
        }
        public void  Execute()
        {
            for(int i=0;i<10;i++)
            {
                bool isAccepted = buffer.Post(i);
                Console.WriteLine("Buffer Count after posting "+i+ ":" + buffer.Count);
                if (isAccepted == false)
                {
                    Console.WriteLine(i+" was not accepted by the buffer block");
                    
                    Console.WriteLine("Making space for new value");
                    int n = buffer.Receive();
                    Console.WriteLine("value :" + n);
                }
            }
            
            
           
            


        }
        public void ExecutePipeline()
        {  //Only and Only Broadcast message can pass the same message to multiple Targets
            //Buffer message will not pass the message to the second  target block, if it has been accepted by the first targetBlock
            buffer.LinkTo(printEvenBlock,n=>n>=0);
            buffer.LinkTo(printOddBlock,n=>n<10);
            for (int i = 0; i < 10; i++)
            {
                bool isAccepted = buffer.Post(i);
                Console.WriteLine("Buffer Count after posting " + i + ":" + buffer.Count);
                if (isAccepted == false)
                {
                    Console.WriteLine(i + " was not accepted by the buffer block");

                    Console.WriteLine("Making space for new value");
                    int n = buffer.Receive();
                    Console.WriteLine("value :" + n);
                }

            }

        }
        private static void printEven(int n)
        {
            Console.WriteLine("Priting Even :"+n);
        }

    }
}
