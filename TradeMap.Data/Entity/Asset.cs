using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Asset
    {
        public Asset()
        {
            ViolationLogs = new HashSet<ViolationLog>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public bool IsDeleted { get; set; }
        public int Type { get; set; }

        public virtual Store Store { get; set; }
        public virtual ICollection<ViolationLog> ViolationLogs { get; set; }
    }
}
