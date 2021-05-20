using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class FloorArea
    {
        public FloorArea()
        {
            InverseReference = new HashSet<FloorArea>();
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public int? FloorId { get; set; }
        public string Name { get; set; }
        public int? ReferenceId { get; set; }

        public virtual Floor Floor { get; set; }
        public virtual FloorArea Reference { get; set; }
        public virtual ICollection<FloorArea> InverseReference { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
