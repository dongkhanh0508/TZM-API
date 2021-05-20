using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Segment
    {
        public Segment()
        {
            Brands = new HashSet<Brand>();
            CustomerSegments = new HashSet<CustomerSegment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<CustomerSegment> CustomerSegments { get; set; }
    }
}
