using System;

namespace TradeMap.Service.DTO.Response
{
    public class HistoryResponse
    {
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public int? BuildingId { get; set; }
        public string ReferenceName { get; set; }
        public Guid? AccountId { get; set; }
        public string AccountName { get; set; }
        public int? Role { get; set; }
        public int? Action { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Geom { get; set; }
        public int? Status { get; set; }
    }
}