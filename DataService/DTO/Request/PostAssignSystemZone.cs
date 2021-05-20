using System;

namespace TradeMap.Service.DTO.Request
{
    public class PostAssignSystemZone
    {
        public Guid AccountId { get; set; }
        public int SystemZoneId { get; set; }
    }
}