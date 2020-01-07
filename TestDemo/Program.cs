using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestDemo.Models;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            HotelSyncTimeByClientFetcherBlock hotelSyncTimeFetcher = new HotelSyncTimeByClientFetcherBlock();
            Task send = hotelSyncTimeFetcher.sendAsync(new Models.UpdatedHotelInfoRequest() { Culture = "en-US", ContentSupplierAccountId = "CapitalOne", Ticks = 637140438000000000 });
            send.Wait();
            hotelSyncTimeFetcher.Complete();
            bool isOutputAvailable = hotelSyncTimeFetcher.OutputAvailableAsync().Result;
            List<UpdatedHotelInfoResponse> updatedHotelList = null;
            while (isOutputAvailable)
            {
                updatedHotelList = hotelSyncTimeFetcher.recieveAsync().Result;
                isOutputAvailable = hotelSyncTimeFetcher.OutputAvailableAsync().Result;
            }
            foreach (var e in updatedHotelList)
            {
                Console.WriteLine("Hotel Id : " + e.HotelId + " LastSyncTime(Ticks) : " + e.Ticks);
            }
            Console.ReadKey();

        }
    }

}
   
      