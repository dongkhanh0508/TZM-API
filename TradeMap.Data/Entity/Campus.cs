using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Campus
    {
        public Campus()
        {
            Buildings = new HashSet<Building>();
            CampusStreetSegments = new HashSet<CampusStreetSegment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<CampusStreetSegment> CampusStreetSegments { get; set; }
    }
}
