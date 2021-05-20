using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class BuildingStreetSegment
    {
        public int BuidingId { get; set; }
        public int StreetSegmentId { get; set; }

        public virtual Building Buiding { get; set; }
        public virtual StreetSegment StreetSegment { get; set; }
    }
}
