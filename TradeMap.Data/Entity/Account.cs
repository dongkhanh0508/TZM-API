using System;
using System.Collections.Generic;

#nullable disable

namespace TradeMap.Data.Entity
{
    public partial class Account
    {
        public Account()
        {
            Histories = new HashSet<History>();
            SystemZones = new HashSet<SystemZone>();
        }

        public Guid Id { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public bool Active { get; set; }
        public string FcmToken { get; set; }
        public DateTime CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<SystemZone> SystemZones { get; set; }
    }
}
