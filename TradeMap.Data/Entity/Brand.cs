using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Brand
    {
        public Brand()
        {
            Accounts = new HashSet<Account>();
            GroupZones = new HashSet<GroupZone>();
            Stores = new HashSet<Store>();
            TradeZoneVersions = new HashSet<TradeZoneVersion>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public int SegmentId { get; set; }

        public virtual Segment Segment { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<GroupZone> GroupZones { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<TradeZoneVersion> TradeZoneVersions { get; set; }
    }
}
