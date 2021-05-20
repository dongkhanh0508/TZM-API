using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class StoreTradeZone
    {
        public int? Id { get; set; } = null;

        public string Name { get; set; }

        public CustomFeatureCollection StoresGeom { get; set; } = new CustomFeatureCollection();

        public CustomFeatureCollection TradeZoneGeom { get; set; } = new CustomFeatureCollection();
    }

    public class StoreTradeZoneResponse
    {
        public List<StoreTradeZone> StoreTradeZone { get; set; }

        public CustomFeatureCollection RemainZones { get; set; }
    }

    public class TradezoneForMap
    {
        public string TimeSlot { get; set; }
        public CustomFeatureCollection Tradezones { get; set; } = new CustomFeatureCollection();
        public CustomFeatureCollection Stores { get; set; } = new CustomFeatureCollection();
        public double? Distance { get; set; } = 0;
    }
    public class ZoneResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Geometry Geom { get; set; }
    }

    public class ZoneCoverageResponse : ZoneResponse
    {
        public double? Coverage { get; set; } = 0;
    }

    public class ZoneWeightResponse : ZoneResponse
    {
        public double Weight { get; set; }
    }



    public class StoreTradeZoneForMap
    {
        public ZoneResponse Store { get; set; }

        public ZoneWeightResponse TradeZone { get; set; }
    }

    public class StoreTradeZoneForMapResponse
    {
        public List<StoreTradeZoneForMap> ListStoreTradeZone = new List<StoreTradeZoneForMap>();

        public Geometry GroupZoneBoundary { get; set; }
    }

}