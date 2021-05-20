using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class PostTradeZoneVerison
    {
        public List<TradezoneRequest> TradeZones { get; set; } = new List<TradezoneRequest>();

        public string Name { get; set; }
        public string Description { get; set; }
        public string DateFilter { get; set; }
        public string TimeSlot { get; set; }
        public bool? IsActive { get; set; } = false;
        public int? BrandId { get; set; }
        public double Distance { get; set; }
    }
}