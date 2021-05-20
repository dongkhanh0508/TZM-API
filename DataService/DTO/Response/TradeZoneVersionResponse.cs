using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class TradeZoneVersionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; } = true;

        public string DateFilter { get; set; }

        public string TimeSlot { get; set; }
    }

    public class TradeZoneVersionDetailResponse : TradeZoneVersionResponse
    {
        public string Description { get; set; }
        public int? BrandId { get; set; }
        public double Distance { get; set; }

        public System.DateTime CreateDate { get; set; }

        public List<TradeZoneVersionForMap> TradeZones { get; set; }
    }
}