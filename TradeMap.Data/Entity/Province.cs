using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Province
    {
        public Province()
        {
            Districts = new HashSet<District>();
        }

        public int Id { get; set; }
        public Geometry Geom { get; set; }
        public int? OsmId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<District> Districts { get; set; }
    }
}
