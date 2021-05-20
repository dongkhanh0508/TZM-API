using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class History
    {
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public Guid AccountId { get; set; }
        public int Action { get; set; }
        public DateTime CreateDate { get; set; }
        public int? BuildingId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Building Building { get; set; }
        public virtual Store Store { get; set; }
    }
}
