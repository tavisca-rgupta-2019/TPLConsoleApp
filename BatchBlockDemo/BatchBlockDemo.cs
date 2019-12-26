using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace BatchBlockDemo
{
    public class BatchBlockDemo
    {
        BatchBlock<int> batch = new BatchBlock<int>(10);

        public void  Execute()
        {
            for(int i=0;i<100;i++)
            {
                batch.Post(i);
            }
            Console.WriteLine("output : {0}",batch.Receive().Sum());
            Console.WriteLine("output : {0}", batch.Receive().Sum());

        }
    }
}
