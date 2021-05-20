using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class PutCampusRequest
    {
        public string Name { get; set; }
        public List<int> StreetSegmentId { get; set; }
    }
}
