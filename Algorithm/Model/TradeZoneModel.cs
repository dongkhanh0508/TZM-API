using System.Collections.Generic;
using TradeMap.Data.Entity;

namespace TradeMap.Algorithm.Model
{
    public class TradeZoneModel
    {
        public Store Store { get; set; }

        public List<SystemZone> ListSystemZones { get; set; }

        public SystemZone SelectedSystemzone { get; set; }

        public double? TotalWeight { get; set; } = 0;

        public List<SystemZone> ListSuitable { get; set; }

        public bool IsSuccess { get; set; } = false;

        public double? MaxTotalWeight { get; set; }

        public List<SystemZone> RemainSystemZones { get; set; }
    }
}