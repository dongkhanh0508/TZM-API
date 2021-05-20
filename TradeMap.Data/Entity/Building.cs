using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Building
    {
        public Building()
        {
            BuildingStreetSegments = new HashSet<BuildingStreetSegment>();
            CustomerSegments = new HashSet<CustomerSegment>();
            Floors = new HashSet<Floor>();
            Histories = new HashSet<History>();
            InverseReference = new HashSet<Building>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? OsmId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Geometry Geom { get; set; }
        public string ImageUrl { get; set; }
        public bool? Active { get; set; }
        public int? NumberOfFloor { get; set; }
        public int? CampusId { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        public int? ReferenceId { get; set; }
        public int? TypeId { get; set; }

        public virtual Campus Campus { get; set; }
        public virtual Building Reference { get; set; }
        public virtual TypeBuilding Type { get; set; }
        public virtual ICollection<BuildingStreetSegment> BuildingStreetSegments { get; set; }
        public virtual ICollection<CustomerSegment> CustomerSegments { get; set; }
        public virtual ICollection<Floor> Floors { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Building> InverseReference { get; set; }
    }
}
