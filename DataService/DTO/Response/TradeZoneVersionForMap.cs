using System;

namespace TradeMap.Service.DTO.Response
{
    public class TradeZoneVersionForMap
    {
        public int TradeZoneId { get; set; }
        public String Name { get; set; }

        public double TotalWeight { get; set; }

        public CustomFeatureCollection TradeZoneGeom { get; set; } = new CustomFeatureCollection();

        public CustomFeatureCollection StoreGeom { get; set; } = new CustomFeatureCollection();
    }
}
