using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class PostBuildingRequest
    {
        public string Name { get; set; }
        public string CoordinateString { get; set; }
        public string ImageUrl { get; set; }
        public int? Type { get; set; }
        public bool? Active { get; set; }
        public int? NumberOfFloor { get; set; }
        public int? CampusId { get; set; }
        public string Address { get; set; }
        public List<int> StreetSegmentIds { get; set; }
        public virtual List<FloorReqest> Floors { get; set; } = new List<FloorReqest>();
    }

    public class FloorReqest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? FloorNumber { get; set; }
        public List<FloorAreaReqest> FloorAreas { get; set; } = new List<FloorAreaReqest>();
    }

    public class FloorAreaReqest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}