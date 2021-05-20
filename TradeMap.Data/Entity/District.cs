using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class District
    {
        public District()
        {
            Wards = new HashSet<Ward>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? OsmId { get; set; }
        public Geometry Geom { get; set; }
        public int ProvinceId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? Population { get; set; }
        public double? Area { get; set; }
        public double? PopulationDensity { get; set; }

        public virtual Province Province { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
