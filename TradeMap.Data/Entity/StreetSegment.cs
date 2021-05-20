using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class StreetSegment
    {
        public StreetSegment()
        {
            BuildingStreetSegments = new HashSet<BuildingStreetSegment>();
            CampusStreetSegments = new HashSet<CampusStreetSegment>();
            StoreStreetSegments = new HashSet<StoreStreetSegment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int WardId { get; set; }
        public int? StreetId { get; set; }

        public virtual Street Street { get; set; }
        public virtual Ward Ward { get; set; }
        public virtual ICollection<BuildingStreetSegment> BuildingStreetSegments { get; set; }
        public virtual ICollection<CampusStreetSegment> CampusStreetSegments { get; set; }
        public virtual ICollection<StoreStreetSegment> StoreStreetSegments { get; set; }
    }
}
