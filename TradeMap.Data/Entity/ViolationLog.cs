using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class ViolationLog
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public Guid AssetId { get; set; }
        public int TypeViolation { get; set; }
        public int Severity { get; set; }
        public Geometry Geom { get; set; }

        public virtual Asset Asset { get; set; }
    }
}
