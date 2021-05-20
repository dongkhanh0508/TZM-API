using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class TradeZoneVersion
    {
        public TradeZoneVersion()
        {
            TradeZones = new HashSet<TradeZone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DateFilter { get; set; }
        public string TimeSlot { get; set; }
        public bool IsActive { get; set; }
        public int BrandId { get; set; }
        public double Distance { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<TradeZone> TradeZones { get; set; }
    }
}
