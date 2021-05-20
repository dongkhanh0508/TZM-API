using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Ward
    {
        public Ward()
        {
            StreetSegments = new HashSet<StreetSegment>();
            SystemZones = new HashSet<SystemZone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? OsmId { get; set; }
        public Geometry Geom { get; set; }
        public int DistrictId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual District District { get; set; }
        public virtual ICollection<StreetSegment> StreetSegments { get; set; }
        public virtual ICollection<SystemZone> SystemZones { get; set; }
    }
}
