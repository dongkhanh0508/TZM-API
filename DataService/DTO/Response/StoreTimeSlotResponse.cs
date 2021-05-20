using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class StoreTimeSlot
    {
        public string TimeSlot { get; set; }

        public CustomFeatureCollection Stores { get; set; } = new CustomFeatureCollection();
    }

    public class StoreTimeSlotResponse
    {
        public List<StoreTimeSlot> ListStore { get; set; } = new List<StoreTimeSlot>()
        {
            new StoreTimeSlot()
            {
                TimeSlot = "1000"
            },
            new StoreTimeSlot()
            {
                TimeSlot = "0100"
            },
            new StoreTimeSlot()
            {
                TimeSlot = "0010"
            }, new StoreTimeSlot()
            {
                TimeSlot = "0001"
            }
        };
    }
}