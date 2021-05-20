using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class GroupZone
    {
        public GroupZone()
        {
            TradeZones = new HashSet<TradeZone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<TradeZone> TradeZones { get; set; }
    }
}
