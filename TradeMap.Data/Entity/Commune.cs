using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Commune
    {
        public Commune()
        {
            StreetSegments = new HashSet<StreetSegment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? OsmId { get; set; }
        public Geometry Geom { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual Ward Ward { get; set; }
        public virtual ICollection<StreetSegment> StreetSegments { get; set; }
    }
}
