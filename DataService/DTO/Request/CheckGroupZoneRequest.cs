using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class CheckGroupZoneRequest
    {
        public List<int> ListZoneId { get; set; }

        public int Type { get; set; }
    }
}
