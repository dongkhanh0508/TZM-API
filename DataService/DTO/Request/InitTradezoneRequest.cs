using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class InitTradezoneRequest
    {
        public List<int> StoresId { get; set; }

        public string TimeSlot { get; set; }

        public double Distance { get; set; }
    }
}