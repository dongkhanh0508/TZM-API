using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class GroupZoneRequest
    {
        public string Name { get; set; }
        public List<int> ListZoneId { get; set; }

        public int Type { get; set; }
    }
}