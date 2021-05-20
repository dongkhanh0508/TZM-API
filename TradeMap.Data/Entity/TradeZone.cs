using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class TradeZone
    {
        public TradeZone()
        {
            StoreTradeZones = new HashSet<StoreTradeZone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public int GroupZoneId { get; set; }
        public double WeightNumber { get; set; }
        public int? TradeZoneVersionId { get; set; }
        public double? Coverage { get; set; }

        public virtual GroupZone GroupZone { get; set; }
        public virtual TradeZoneVersion TradeZoneVersion { get; set; }
        public virtual ICollection<StoreTradeZone> StoreTradeZones { get; set; }
    }
}
