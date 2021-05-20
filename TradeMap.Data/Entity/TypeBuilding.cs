using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class TypeBuilding
    {
        public TypeBuilding()
        {
            Buildings = new HashSet<Building>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
    }
}
