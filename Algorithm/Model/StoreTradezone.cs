using NetTopologySuite.Geometries;
using System.Collections.Generic;
using TradeMap.Data.Entity;

namespace TradeMap.Algorithm.Model
{
    public class StoreTradezone
    {
        public Store Store { get; set; }

        public List<SystemZone> ListSystemzone { get; set; }

        public Geometry TradeZoneGeom { get; set; }

        public double? TotalWeight { get; set; }

        public int GroupZoneId { get; set; }

        public List<SystemZone> RemainZone { get; set; } = new List<SystemZone>(); 



    }
}