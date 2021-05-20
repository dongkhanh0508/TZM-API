using System.Collections.Generic;
using TradeMap.Data.Entity;

namespace TradeMap.Service.Helpers
{
    public class TradeZoneExcuteModel
    {
        public int Id { get; set; }

        public List<SystemZone> Systemzones { get; set; } = new List<SystemZone>();

        public List<Store> Stores { get; set; } = new List<Store>();
    }
}