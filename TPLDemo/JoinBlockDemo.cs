using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDemo
{
    public class JoinBlockDemo
    {
        BufferBlock<int> firstBuffer = new BufferBlock<int>();
        BufferBlock<int> secondBuffer = new BufferBlock<int>();
        BufferBlock<char> thirdBuffer = new BufferBlock<char>();

        JoinBlock<int,int,char> joinBlock = new JoinBlock<int, int, char>();
        ActionBlock<Tuple<int, int, char>> tuplePrinting = new ActionBlock<Tuple<int, int, char>>(tuple => printTuple(tuple));
        public async Task Execute()
        {

            firstBuffer.LinkTo(joinBlock.Target1, new DataflowLinkOptions() { PropagateCompletion = true });
            secondBuffer.LinkTo(joinBlock.Target2, new DataflowLinkOptions() { PropagateCompletion = true });
            thirdBuffer.LinkTo(joinBlock.Target3, new DataflowLinkOptions() { PropagateCompletion = true });
            joinBlock.LinkTo(tuplePrinting, new DataflowLinkOptions() { PropagateCompletion = true });

            var populatingFirstBuffer = Task.Factory.StartNew(async () => {
                for (int i = 11; i <= 20; i++)
                {
                    await firstBuffer.SendAsync(i);
                }

            });
            var populatingSecondBuffer = Task.Factory.StartNew(async () => {
                for (int i = 1; i <= 10; i++)
                {
                    await secondBuffer.SendAsync(i);
                }

            });
            var populatingThirdBuffer = Task.Factory.StartNew(async () => {
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                        await thirdBuffer.SendAsync('-');
                    else
                        await thirdBuffer.SendAsync('+');
                }

            });
            await Task.WhenAll(populatingFirstBuffer, populatingSecondBuffer, populatingThirdBuffer).ContinueWith(task =>
              {
                  firstBuffer.Complete();
                  secondBuffer.Complete();
                  thirdBuffer.Complete();
              }
            );
            try
            {
                await tuplePrinting.Completion;
                await tuplePrinting.Completion.ContinueWith(task => Console.WriteLine("Pipleline Completed"),TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(AggregateException ex)
            {
                Console.WriteLine("Exception Occured : "+ex.InnerException.Message);
            }

        }

        private static void printTuple(Tuple<int,int,char> tuple)
        {
            Console.WriteLine("Tuple :");
            switch (tuple.Item3)
            {
                case '+':
                    Console.WriteLine("{0}+{1}={2}", tuple.Item1, tuple.Item2, tuple.Item1 + tuple.Item2);
                    break;
                case '-':
                    Console.WriteLine("{0}-{1}={2}", tuple.Item1, tuple.Item2, tuple.Item1 - tuple.Item2);
                    break;
                default:
                    Console.WriteLine("invalid Operation");
                    break;
            }

        }
    
    }
        
}
