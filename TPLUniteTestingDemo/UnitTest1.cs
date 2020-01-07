using System;
using System.Threading.Tasks;
using TestDemo;
using TPLDemo;
using Xunit;
using Nito.AsyncEx.UnitTests;
using TestDemo.Models;
using System.Collections.Generic;

namespace TPLUniteTestingDemo
{
    public class UnitTest1
    {
        [Fact]
        public async void Should_Return_Hotels_With_SyncTime_Ahead_Of_LastSyncTime_Of_Client()
        {
            HotelSyncTimeByClientFetcherBlock hotelSyncTimeFetcher = new HotelSyncTimeByClientFetcherBlock();
            await hotelSyncTimeFetcher.sendAsync(new UpdatedHotelInfoRequest() { Culture = "en-US", ContentSupplierAccountId = "CapitalOne", Ticks = 637108470000000000 });
            hotelSyncTimeFetcher.Complete();
            bool isOutputAvailable = await hotelSyncTimeFetcher.OutputAvailableAsync();
            List<UpdatedHotelInfoResponse> updatedHotelList = null;

            while (isOutputAvailable)
            {
                updatedHotelList = await hotelSyncTimeFetcher.recieveAsync();
                isOutputAvailable = await hotelSyncTimeFetcher.OutputAvailableAsync();
            }

            foreach (var e in updatedHotelList)
            {
                Assert.NotInRange<long>(e.Ticks, 000000000000000000, 637108470000000000);
            }
        }


        [Fact]
        public async void Should_Not_Return_Hotels_On_Invokation_of_Cancellation_Token()
        {
            HotelSyncTimeByClientFetcherBlock hotelSyncTimeFetcher = new HotelSyncTimeByClientFetcherBlock();
            await hotelSyncTimeFetcher.sendAsync(new UpdatedHotelInfoRequest() { Culture = "en-US", ContentSupplierAccountId = "CapitalOne", Ticks = 637108470000000000 });
            hotelSyncTimeFetcher.Complete();
            hotelSyncTimeFetcher.Cancel();

            bool isOutputAvailable = await hotelSyncTimeFetcher.OutputAvailableAsync();

            Assert.False(isOutputAvailable);
        }

        [Fact]
        public async void Should_Throw_Exception_When_Client_LastSyncTime_is_Zero()
        {
            HotelSyncTimeByClientFetcherBlock hotelSyncTimeFetcher = new HotelSyncTimeByClientFetcherBlock();
            await hotelSyncTimeFetcher.sendAsync(new UpdatedHotelInfoRequest() { Culture = "en-US", ContentSupplierAccountId = "CapitalOne", Ticks = 000000000000000000 });

            hotelSyncTimeFetcher.Complete();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => hotelSyncTimeFetcher.Completion);


        }

    }
       
}
