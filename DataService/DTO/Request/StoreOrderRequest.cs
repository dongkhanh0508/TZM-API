using System;

namespace TradeMap.Service.DTO.Request
{
    public class StoreOrderRequest
    {
        public string CoordinateString { get; set; }
        public DateTime TimeOrder { get; set; }

        public int DateOrder { get; set; }
    }
}