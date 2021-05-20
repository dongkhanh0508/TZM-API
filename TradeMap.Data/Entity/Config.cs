using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Config
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public int Version { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
    }
}
