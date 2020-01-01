using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestDemo
{

    class DemoBlock
    {
        BufferBlock<int> sourceBlock = new BufferBlock<int>();
        TransformBlock<int, int> targetBlock = new TransformBlock<int, int>(element =>
        {
            Thread.Sleep(1000);
            return element * 2;
        });

        public async Task Execute()
        {
            sourceBlock.LinkTo(targetBlock, new DataflowLinkOptions { PropagateCompletion = true });

            //feed some elements into the buffer block
            for (int i = 1; i <= 10; i++)
            {
                await sourceBlock.SendAsync(i);
            }

            sourceBlock.Complete();
            
            bool isOutputAvailable = await targetBlock.OutputAvailableAsync();
            while(isOutputAvailable)
            {
                int value = await targetBlock.ReceiveAsync();
                
                isOutputAvailable = await targetBlock.OutputAvailableAsync();
            }


            await targetBlock.Completion.ContinueWith(_ =>
            {
                Console.WriteLine("Target Block Completed");//notify completion of the target block
            });



        }


    }
}
