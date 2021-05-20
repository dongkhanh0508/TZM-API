using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class StoreTradeZone
    {
        public int StoreId { get; set; }
        public int TradeZoneId { get; set; }

        public virtual Store Store { get; set; }
        public virtual TradeZone TradeZone { get; set; }
    }
}
