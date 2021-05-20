using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Floor
    {
        public Floor()
        {
            FloorAreas = new HashSet<FloorArea>();
            InverseReference = new HashSet<Floor>();
        }

        public int Id { get; set; }
        public int? FloorNumber { get; set; }
        public int? BuildingId { get; set; }
        public string Name { get; set; }
        public int? ReferenceId { get; set; }

        public virtual Building Building { get; set; }
        public virtual Floor Reference { get; set; }
        public virtual ICollection<FloorArea> FloorAreas { get; set; }
        public virtual ICollection<Floor> InverseReference { get; set; }
    }
}
