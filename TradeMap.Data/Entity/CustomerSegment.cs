using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class CustomerSegment
    {
        public int BuildingId { get; set; }
        public int SegmentId { get; set; }
        public string TimeSlot { get; set; }
        public int? PrimaryAge { get; set; }
        public int PotentialCustomers { get; set; }

        public virtual Building Building { get; set; }
        public virtual Segment Segment { get; set; }
    }
}
