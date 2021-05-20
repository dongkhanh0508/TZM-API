using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class StoreStreetSegment
    {
        public int StoreId { get; set; }
        public int StreetSegmentId { get; set; }

        public virtual Store Store { get; set; }
        public virtual StreetSegment StreetSegment { get; set; }
    }
}
