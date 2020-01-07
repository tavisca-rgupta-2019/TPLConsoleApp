using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace BatchedJoinBlockDemo
{
    class BatchedJoinBlockDemo
    {
        public void Execute()
        {
            Func<int, int> DoWork = n =>
             {
                 if (n < 0)
                     throw new ArgumentOutOfRangeException();
                 return n;
             };

            var batchJoinBlock = new BatchedJoinBlock<int, Exception>(7);

            foreach(int i in new int[] { -1,-2,-45,-34,76,56,34,66,22,33,55,-34,-98,-6,0,89})
            {
                try
                {
                    // Post the result of the worker to the 
                    // first target of the block.
                    batchJoinBlock.Target1.Post(DoWork(i));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // If an error occurred, post the Exception to the 
                    // second target of the block.
                    batchJoinBlock.Target2.Post(e);
                }
            }

            var results = batchJoinBlock.Receive();
            //The combined size of both the list will be equal to the specified batch size
            if(results.Item1.Count+results.Item2.Count == 7)
                Console.WriteLine("batch size verified");
            foreach (int n in results.Item1)
            {
                Console.WriteLine(n);
            }
            // Print failures.
            foreach (Exception e in results.Item2)
            {
                Console.WriteLine(e.Message);
            }





        }
        
    }
}
