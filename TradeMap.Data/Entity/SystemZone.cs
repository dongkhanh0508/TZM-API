using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class SystemZone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public double? WeightNumber { get; set; }
        public Guid? AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Ward Ward { get; set; }
    }
}
