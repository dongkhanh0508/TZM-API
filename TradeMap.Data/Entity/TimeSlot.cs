using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class TimeSlot
    {
        public TimeSlot()
        {
            TradeZones = new HashSet<TradeZone>();
        }

        public int Id { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }

        public virtual ICollection<TradeZone> TradeZones { get; set; }
    }
}
