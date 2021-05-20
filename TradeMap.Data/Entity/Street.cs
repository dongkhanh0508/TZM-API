using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Street
    {
        public Street()
        {
            StreetSegments = new HashSet<StreetSegment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public string Type { get; set; }
        public int? OsmId { get; set; }
        public bool? Oneway { get; set; }
        public int? Maxspeed { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<StreetSegment> StreetSegments { get; set; }
    }
}
