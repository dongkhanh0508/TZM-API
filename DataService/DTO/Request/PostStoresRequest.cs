using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class PostStoresRequest
    {
        public string Name { get; set; }
        public string TimeSlot { get; set; }
        public int BrandId { get; set; }
        public string Address { get; set; }
        public int FloorAreaId { get; set; }
        public string CoordinateString { get; set; }
        public List<int> StreetSegmentIds { get; set; }
        public string ImageUrl { get; set; }
        public int? AbilityToServe { get; set; }
    }
}