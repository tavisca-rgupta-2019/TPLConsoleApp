using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestDemo.Models;

namespace TestDemo
{
   
    public class HotelSyncTimeByClientFetcherBlock
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken token;
        private TransformBlock<UpdatedHotelInfoRequest, List<UpdatedHotelInfoResponse>> HotelFetcherBlock;

        public HotelSyncTimeByClientFetcherBlock()
        {
            token = cts.Token;
            HotelFetcherBlock = new TransformBlock<UpdatedHotelInfoRequest, List<UpdatedHotelInfoResponse>>((request) =>
            getHotelsSyncTime(request), new ExecutionDataflowBlockOptions() { CancellationToken = token });

        }


        private List<UpdatedHotelInfoResponse> getHotelsSyncTime(UpdatedHotelInfoRequest request)
        {
            if (request.Ticks == 0)
                throw new ArgumentOutOfRangeException();
            Thread.Sleep(6000);
            List<UpdatedHotelInfoResponse> hotelList = new List<UpdatedHotelInfoResponse>();

            for (int i = 1; i <= 10; i++)
            {
                hotelList.Add(new UpdatedHotelInfoResponse()
                {
                    HotelId = i.ToString(),
                    Ticks = 637140438000000000,
                    SupplierHotelId = i.ToString() + "0",
                    SupplierFamily = "EPS Rapid"


                });
            }

            return hotelList;


        }
        public async Task sendAsync(UpdatedHotelInfoRequest requestObject)
        {
            await HotelFetcherBlock.SendAsync(requestObject);
        }

        public async Task<List<UpdatedHotelInfoResponse>> recieveAsync()
        {
            var hotelInfo = await HotelFetcherBlock.ReceiveAsync();
            return hotelInfo;
        }
        public void Complete()
        {
            HotelFetcherBlock.Complete();
        }
        public void Cancel()
        {
            cts.Cancel();
        }
        public Task Completion
        {
            get
            {
                return HotelFetcherBlock.Completion;
            }
        }
        public async Task<bool> OutputAvailableAsync()
        {
            return await HotelFetcherBlock.OutputAvailableAsync();
        }



    }
}
