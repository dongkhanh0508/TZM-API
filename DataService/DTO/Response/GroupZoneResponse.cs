using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class GroupZoneResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public int? BrandId { get; set; }

        public bool? IsDeleteActive { get; set; }
    }

    public class StoreGroupZone
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Geometry Geom { get; set; }

    }

    public class TradeZoneGroupZone
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Geometry Geom { get; set; }


        public double TotalWeight { get; set; }
    }

    public class GroupStoreTradeZoneResponse : GroupZoneResponse
    {
        public List<StoreGroupZone> Stores { get; set; }

        public List<TradeZoneGroupZone> TradeZones { get; set; }
    }
}