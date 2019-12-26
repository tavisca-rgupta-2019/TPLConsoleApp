using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace JoinBlockDemo
{
    public class JoinBlockDemo
    {
        public void Execute()
        {
            //JoinBlock collect input elements and propagate out System.Tuple<T1,T2> or System.Tuple<T1,T2,T3> objects that contain those elements. 
            //Target1,Target2and Target3 are the ITargetBlock to whom we need to pass the inputs in order to make a tuple out of them
            var joinBlock = new JoinBlock<int, int, char>();
            joinBlock.Target1.Post(10);
            joinBlock.Target2.Post(20);
            joinBlock.Target3.Post('-');

            Tuple<int, int, char> data = joinBlock.Receive();
            switch(data.Item3)
            {
                case '+': Console.WriteLine("{0}+{1}={2}",data.Item1,data.Item2,data.Item1+data.Item2);
                          break;
                case '-': Console.WriteLine("{0}-{1}={2}", data.Item1, data.Item2, data.Item1 - data.Item2);
                         break;
                default : Console.WriteLine("invalid Operation");
                            break;
            }
        }
    
    }
        
}
