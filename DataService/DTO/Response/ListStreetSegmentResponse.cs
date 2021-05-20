using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class ListStreetSegmentResponse
    {
        public List<StreetSegmentResponse> ListStreetSegment { get; set; } = new List<StreetSegmentResponse>();
    }
}