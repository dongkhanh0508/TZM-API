using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class CampusStreetSegment
    {
        public int CampusId { get; set; }
        public int StreetSegmentId { get; set; }

        public virtual Campus Campus { get; set; }
        public virtual StreetSegment StreetSegment { get; set; }
    }
}
