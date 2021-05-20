using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Area
    {
        public Area()
        {
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? FloorId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? Active { get; set; }

        public virtual FloorArea Floor { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
