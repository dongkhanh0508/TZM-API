using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Store
    {
        public Store()
        {
            Assets = new HashSet<Asset>();
            Histories = new HashSet<History>();
            InverseReference = new HashSet<Store>();
            StoreStreetSegments = new HashSet<StoreStreetSegment>();
            StoreTradeZones = new HashSet<StoreTradeZone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string Address { get; set; }
        public int? OsmId { get; set; }
        public int? FloorAreaId { get; set; }
        public string ImageUrl { get; set; }
        public int? Status { get; set; }
        public int? ReferenceId { get; set; }
        public int? AbilityToServe { get; set; }
        public string TimeSlot { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual FloorArea FloorArea { get; set; }
        public virtual Store Reference { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Store> InverseReference { get; set; }
        public virtual ICollection<StoreStreetSegment> StoreStreetSegments { get; set; }
        public virtual ICollection<StoreTradeZone> StoreTradeZones { get; set; }
    }
}
